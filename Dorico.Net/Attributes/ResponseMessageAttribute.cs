namespace DoricoNet.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ResponseMessageAttribute(string messageId) : Attribute
{
    public string MessageId { get; } = messageId;
}
