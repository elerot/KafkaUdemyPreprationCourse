namespace KafkaConsumer.Events;

internal class OrderCreatedEvent
{
    public string OrderCode { get; set; } = null!;
    public decimal TotalPrice { get; set; }
    public int UserId { get; set; }

    public override string ToString()
    {
        return $"{OrderCode} -{TotalPrice} - {UserId}";
    }
}