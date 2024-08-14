using DS_Project.GateWay.Utils;
using System.Collections.Concurrent;

namespace DS_Project.GateWay.Services
{
    public class RequestQueueService
    {
        private readonly ConcurrentQueue<HttpRequestMessage> _requestMessagesQueue = new();
        private readonly HttpClient _httpClient = new();
        private const int TimeoutInSeconds = 10;
        private static readonly object locker = new();
        private readonly CircuitBreaker _circuitBreaker;

        public RequestQueueService()
        {
            _circuitBreaker = CircuitBreaker.Instance;
        }

        public void AddRequestToQueue(HttpRequestMessage httpRequestMessage)
        {
            _requestMessagesQueue.Enqueue(httpRequestMessage);
        }

        public void StartWorker()
        {
            new Thread(Start).Start();
            Console.WriteLine("Thread started");
        }

        private async void Start(object? state)
        {
            while (true)
            {
                lock (locker)
                {
                    if (!_requestMessagesQueue.TryPeek(out var req))
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        continue;
                    }

                    try
                    {
                        var res = _httpClient.Send(req);
                        if (res.IsSuccessStatusCode)
                        {
                            _requestMessagesQueue.TryDequeue(out _);
                            _circuitBreaker.ResetFailureCount();
                        }
                        else
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(TimeoutInSeconds));
                        }
                    }
                    catch (Exception)
                    {
                        var reqClone = HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req).GetAwaiter().GetResult();
                        _requestMessagesQueue.TryDequeue(out _);
                        _requestMessagesQueue.Enqueue(reqClone);

                        Thread.Sleep(TimeSpan.FromSeconds(TimeoutInSeconds));
                    }
                }
            }
        }
    }
}
