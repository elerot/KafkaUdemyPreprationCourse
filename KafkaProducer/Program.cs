// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaProducer;
using KafkaProducer.Events;

Console.WriteLine("Producer");

await CreateTopic();
await SendMessageWithType();

async Task CreateTopic()
{
    using var adminClient =
        new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:9094" }).Build();
    try
    {
        await adminClient.CreateTopicsAsync(new[]
        {
            new TopicSpecification { Name = "topic.with.type", ReplicationFactor = 1, NumPartitions = 12 }
        });
    }
    catch (CreateTopicsException e)
    {
        Console.WriteLine(e.Message);
    }
}

async Task SendMessageWithNullKey()
{
    var config = new ProducerConfig { BootstrapServers = "localhost:9094" };

    using var producer = new ProducerBuilder<Null, string>(config).Build();

    foreach (var item in Enumerable.Range(1, 100).ToList())
    {
        var result = await producer.ProduceAsync("mytopicwithkey",
            new Message<Null, string> { Value = $"a log message {item}", Timestamp = Timestamp.Default });

        foreach (var propertyInfo in result.GetType().GetProperties())
            Console.WriteLine($"{propertyInfo.Name} = {propertyInfo.GetValue(result)}");
        Console.WriteLine("---------------");
        await Task.Delay(500);
    }
}

async Task SendMessageWithKey()
{
    var config = new ProducerConfig { BootstrapServers = "localhost:9094" };

    using var producer = new ProducerBuilder<int, string>(config).Build();

    foreach (var item in Enumerable.Range(1, 10).ToList())
    {
        var result = await producer.ProduceAsync("mytopicwithkey",
            new Message<int, string> { Key = item, Value = $"a log message {item}", Timestamp = Timestamp.Default });

        foreach (var propertyInfo in result.GetType().GetProperties())
            Console.WriteLine($"{propertyInfo.Name} = {propertyInfo.GetValue(result)}");
        Console.WriteLine("---------------");
        await Task.Delay(500);
    }
}

async Task SendMessageWithPartitionName()
{
    var config = new ProducerConfig { BootstrapServers = "localhost:9094" };

    using var producer = new ProducerBuilder<int, string>(config).Build();

    foreach (var item in Enumerable.Range(1, 10).ToList())
    {
        var topicPart = new TopicPartition("mytopicwithkey", new Partition(7));

        var result = await producer.ProduceAsync(topicPart,
            new Message<int, string> { Key = item, Value = $"a log message {item}", Timestamp = Timestamp.Default });

        foreach (var propertyInfo in result.GetType().GetProperties())
            Console.WriteLine($"{propertyInfo.Name} = {propertyInfo.GetValue(result)}");
        Console.WriteLine("---------------");
        await Task.Delay(500);
    }
}

async Task SendMessageWithType()
{

    var config = new ProducerConfig { BootstrapServers = "localhost:9094" };

    using var producer = new ProducerBuilder<int, OrderCreatedEvent>(config)
        .SetValueSerializer(new CustomValueSerializer<OrderCreatedEvent>())
        .Build();

    foreach (var item in Enumerable.Range(1, 10).ToList())
    {


        var message = new Message<int, OrderCreatedEvent>
        {
            Key = 10,
            Value = new OrderCreatedEvent() { OrderCode = "abc", TotalPrice = 100, UserId = 10 }
        };



        var result = await producer.ProduceAsync("topic.with.type", message);



        foreach (var propertyInfo in result.GetType().GetProperties())
            Console.WriteLine($"{propertyInfo.Name} = {propertyInfo.GetValue(result)}");
        Console.WriteLine("---------------");
        await Task.Delay(500);
    }
}