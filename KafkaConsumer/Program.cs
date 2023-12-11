// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using KafkaConsumer.Events;

Console.WriteLine("Message is reading");

var config = new ConsumerConfig
{
    BootstrapServers = "localhost:9094",
    GroupId = "mygroup-l",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

//var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

var consumer = new ConsumerBuilder<int, OrderCreatedEvent>(config)
    .SetValueDeserializer(new CustomValueDeSerializer<OrderCreatedEvent>())
    .Build();
consumer.Subscribe("topic.with.type");

while (true)
{
    var consumeResult = consumer.Consume();

    Console.WriteLine(consumeResult.Message.Value);

    //“at least once” delivery semantics
    try
    {
        consumer.Commit(consumeResult);
    }
    catch (KafkaException e)
    {
        Console.WriteLine($"Commit error: {e.Error.Reason}");
    }
}

//consumer.Close();