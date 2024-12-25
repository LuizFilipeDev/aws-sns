using Newtonsoft.Json;

namespace aws_sns
{
    public class Message
    {
        public string Default { get; set; } = string.Empty;
        public string? Email { get; set; } = default;
        [JsonProperty("email-json")]
        public string? EmailJson { get; set; } = default;
        public string? Http { get; set; } = default;
        public string? Https { get; set; } = default;
        public string? Sqs { get; set; } = default;
    }
}
