namespace DS_Project.GateWay.Services
{
    public class CircuitBreaker
    {
        private static readonly object syncRoot = new();
        private static int failureCount = 0;
        private static readonly int N = 3;
        private static volatile CircuitBreaker? instance = null;

        private static bool IsStateOpened = false;

        public static CircuitBreaker Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new CircuitBreaker();
                    }
                }
                return instance;
            }
        }

        private CircuitBreaker() { }

        public bool IsOpened()
        {
            return IsStateOpened;
        }

        public void IncrementFailureCount()
        {
            failureCount++;
            IsStateOpened = failureCount >= N;
        }

        public void ResetFailureCount()
        {
            failureCount = 0;
            IsStateOpened = false;
        }
    }
}
