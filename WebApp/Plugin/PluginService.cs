using Interfaces;
using Confluent.Kafka;

namespace Plugin
{
    public class PluginService : IPluginService
    {
        public string Test()
        {
            return "Tested!";
        }

        public string Produce()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:29092"
            };
            
            var result = ""; 
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                result = producer.ProduceAsync("my-topic", new Message<Null, string> { Value="hello world" }).Result.Value;
            }
            return result;
        }

        public string Consume()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            var result = "";
            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("my-topic");
                while (true)
                {
                    var consumeResult = consumer.Consume(5000);
                    result = consumeResult.Message.Value;
                    break;
                }
                consumer.Close();
            }
            return result;
        }
    }
}