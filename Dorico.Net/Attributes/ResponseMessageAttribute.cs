namespace DoricoNet.Attributes;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class ResponseMessageAttribute : Attribute
{
    public string MessageId { get; }

    public ResponseMessageAttribute(string messageId)
    {
        MessageId = messageId;
    }
}
