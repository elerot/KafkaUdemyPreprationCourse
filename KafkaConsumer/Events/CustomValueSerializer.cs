using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace KafkaConsumer.Events
{
    internal class CustomValueDeSerializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data)!;
        }
    }
}
