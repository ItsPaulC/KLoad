using System.Text.Json;
using Confluent.Kafka;

namespace KLoad.Consumer
{
    /// <summary>
    /// Custom deserializer for the Employee type.
    /// </summary>
    public class EmployeeDeserializer : IDeserializer<Employee>
    {
        public Employee Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<Employee>(data) ?? new Employee();
        }
    }

    /// <summary>
    /// The entry point for the KLoad.Consumer application.
    /// This application initializes the Kafka consumer and processes incoming Employee messages.
    /// </summary>
    public class Program
    {
        private const string KafkaBroker = "localhost:9092"; // Kafka broker address
        private const string Topic = "employee_topic"; // Topic to subscribe to

        public static async Task Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = KafkaBroker,
                GroupId = "employee_consumer_group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, Employee>(config)
                .SetValueDeserializer(new EmployeeDeserializer())
                .Build();
            consumer.Subscribe(Topic);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true; // Prevent the process from terminating.
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = consumer.Consume(cts.Token);
                        var employee = cr.Message.Value;

                        Console.WriteLine($"Consumed Employee: Id={employee.Id}, Name={employee.Name}, Position={employee.Position}");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                consumer.Close();
            }
        }
    }
}
