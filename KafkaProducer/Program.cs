// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;
using Confluent.Kafka.Admin;

Console.WriteLine("Producer");
var topicName = "topic-at-least-one";
await CreateTopic();
await SendWithAtLeastOneSemantic();
Console.WriteLine("message has send to topic");

async Task CreateTopic()
{
    using var adminClient =
        new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost:9094" }).Build();
    try
    {
        await adminClient.CreateTopicsAsync(new[]
        {
            new TopicSpecification { Name = topicName, ReplicationFactor = 1, NumPartitions = 12 }
        });
    }
    catch (CreateTopicsException e)
    {
        Console.WriteLine(e.Message);
    }
}


Task SendWithAtMostOneSemantic()
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9094",
        MessageSendMaxRetries = 0, //  retry must be 0 for at most one
        Acks = Acks.All // configuration can change according to your needs
    };

    using var producer = new ProducerBuilder<int, string>(config).Build();


    //1.step  record to database

    //2.step send to kafka message

    producer.ProduceAsync(topicName,
        new Message<int, string> { Key = 1, Value = "a  1. message", Timestamp = Timestamp.Default }).ContinueWith(x =>
    {
        // handle error
        Console.WriteLine(x.Result.Value);
    });

    return Task.CompletedTask;
}


async Task SendWithAtLeastOneSemantic()
{
    var config = new ProducerConfig
    {
        BootstrapServers = "localhost:9094",
        MessageSendMaxRetries = 10, //  retry must be different from 0 for at least one
        Acks = Acks.All // configuration can change according to your needs
    };

    using var producer = new ProducerBuilder<int, string>(config).Build();

    //1.step send to kafka message
    await producer.ProduceAsync(topicName,
        new Message<int, string> { Key = 1, Value = "a  1.message", Timestamp = Timestamp.Default });

    //2.step  record to database
}