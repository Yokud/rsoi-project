using Confluent.Kafka;

namespace DS_Project.Rentals
{
    public class LogsProducer
    {
        private readonly IProducer<Null, string> _producer;

        public LogsProducer()
        {
            var config = new ProducerConfig
            {
                // User-specific properties that you must set
                BootstrapServers = "broker-kafka:29092",
                // Fixed properties
                Acks = Acks.All,

            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task<DeliveryResult<Null, string>> Produce(string message)
        {
            return await _producer.ProduceAsync("logs", new Message<Null, string>
            {
                Value = message
            });
        }
    }
}
