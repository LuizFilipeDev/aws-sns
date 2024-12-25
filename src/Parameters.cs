namespace aws_sns;

public class Parameter
{
    public string DefaultMessage { get; set; } = string.Empty;
    public Message Message { get; set; } = new();
    public Guid MessageGroupId { get; set; } = default;
    public string? Subject { get; set; } = default;
    public string TopicArn { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

}
