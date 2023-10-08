using Confluent.Kafka;

namespace ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:29092", // Change to your Kafka broker address
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var topic = "MyTopic"; // Replace with your Kafka topic name
                try
                {
                    var deliveryReport = await producer.ProduceAsync(topic, new Message<Null, string> { Value = "Hello, Kafka!" });
                    Console.WriteLine($"Message delivered to {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}