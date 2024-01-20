// See https://aka.ms/new-console-template for more information

using Confluent.Kafka;

Console.WriteLine("Message is reading");


ConsumerWithAtMostOneSemantic();

void ConsumerWithAtMostOneSemantic()
{
    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9094",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = true,
        GroupId = "a"
    };

    var consumer = new ConsumerBuilder<int, string>(config).Build();


    consumer.Subscribe("topic-at-most-one");

    while (true)
    {
        var consumeResult = consumer.Consume();
        // record to database
        Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
    }
}


void ConsumerWithAtLeastOneSemantic()
{
    var config = new ConsumerConfig
    {
        BootstrapServers = "localhost:9094",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false,
        GroupId = "a"
    };

    var consumer = new ConsumerBuilder<int, string>(config).Build();


    consumer.Subscribe("topic-at-least-one");

    while (true)
    {
        var consumeResult = consumer.Consume();
        // 1.step record to database
        Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
        // 2.step commit
        consumer.Commit(consumeResult);
    }
}