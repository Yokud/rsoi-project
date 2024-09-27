using Confluent.Kafka;

namespace DS_Project.Statistics
{
    public class LogsConsumer
    {
        public IConsumer<Null, string> _consumer;

        public LogsConsumer()
        {
            var config = new ConsumerConfig
            {
                // User-specific properties that you must set
                BootstrapServers = "broker-kafka:29092",
                GroupId = "statistics",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _consumer.Subscribe("logs");
        }

        public string Consume()
        {
            try
            {
                var res = _consumer.Consume(10000);
                if (res == null)
                {
                    return "plz stop";
                }
                _consumer.Commit(res);
                return res.Message.Value;
            }
            catch
            {
                return "plz stop";
            }
        }
    }
}
