using aws_sns;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

//Create a logger factory to configure logging behavior.
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<AwsSns>();
var awsSns = new AwsSns(
    new Amazon.SimpleNotificationService.AmazonSimpleNotificationServiceClient(),
    logger
);

//Define constants for AWS configuration and SNS topic details.
const string AWS_REGION = "us-east-1";
const string AWS_ACCOUNT_ID = "123456789012";
const string AWS_SNS_TOPIC = "topic-test.fifo";

//Construct the SNS Topic ARN (Amazon Resource Name) using region, account ID, and topic name.
//The ARN uniquely identifies the SNS topic.
const string TOPIC_ARN = $"arn:aws:sns:{AWS_REGION}:{AWS_ACCOUNT_ID}:{AWS_SNS_TOPIC}";

//Create sample content to be used as the message payload.
var genericContent = new
{
    UserId = Guid.NewGuid(),
    UserName = $"User-{Guid.NewGuid()}",
    Age = Random.Shared.Next(18, 100),
};

var genericContentJson = 
    JsonConvert.SerializeObject(genericContent);

//Create a Message object containing the serialized JSON for different delivery protocols.
//Each property represents a different protocol SNS might deliver to.
var message = new Message()
{
   Email = genericContentJson,
   EmailJson = genericContentJson,
   Http = genericContentJson,
   Https = genericContentJson,
   Sqs = genericContentJson
};

await awsSns.PublishAsync(
    new Parameter
    {
        TopicArn = TOPIC_ARN,
        DefaultMessage = genericContentJson,
        MessageGroupId = Guid.NewGuid()
    },
    AwsSnsType.SqsFifo
);