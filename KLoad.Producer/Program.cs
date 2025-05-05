using System.Text.Json;
using Confluent.Kafka;

namespace KLoad.Producer
{
    /// <summary>
    /// Custom serializer for the Employee type.
    /// </summary>
    public class EmployeeSerializer : ISerializer<Employee>
    {
        public byte[] Serialize(Employee data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }

    /// <summary>
    /// The entry point for the KLoad.Producer application.
    /// This application initializes the Kafka producer and sends Employee messages.
    /// </summary>
    public class Program
    {
        private const string KafkaBroker = "localhost:9092";
        private const string Topic = "employee_topic";

        public static async Task Main(string[] args)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = KafkaBroker
            };

            using IProducer<string, Employee> producer = new ProducerBuilder<string, Employee>(config)
                .SetValueSerializer(new EmployeeSerializer())
                .Build();
                
            for (int i = 1; i <= 10; i++)
            {
                Employee employee = new Employee
                {
                    Id = i,
                    Name = $"Employee {i}",
                    Position = "Developer"
                };

                Message<string, Employee> message = new Message<string, Employee>
                {
                    Key = employee.Id.ToString(),
                    Value = employee
                };

                try
                {
                    var deliveryResult = await producer.ProduceAsync(Topic, message);
                    Console.WriteLine($"Delivered '{deliveryResult.Value}' to '{deliveryResult.TopicPartitionOffset}'");
                }
                catch (ProduceException<string, Employee> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}
