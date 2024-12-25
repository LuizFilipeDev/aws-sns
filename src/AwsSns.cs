using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace aws_sns;

public class AwsSns(
    AmazonSimpleNotificationServiceClient snsClient,
    ILogger<AwsSns> logger)
{
    private readonly AmazonSimpleNotificationServiceClient _snsClient = snsClient;
    private readonly ILogger<AwsSns> _logger = logger;

    private const string JSON_MESSAGE_FORMAT = "json";
    private const string SUBJECT_EMPTY = "SUBJECT_EMPTY";

    /// <summary>
    /// Publishes a message to AWS SNS based on the specified SNS type.
    /// </summary>
    /// <param name="parameter">Parameters containing message details, such as TopicArn, Message, and Subject.</param>
    /// <param name="awsSnsType">The type of SNS endpoint (e.g., SqsFifo, Sms, Http, etc.).</param>
    /// <returns>A boolean indicating whether the message was successfully published.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the parameter is null.</exception>
    public async Task<bool> PublishAsync(
        Parameter parameter,
        AwsSnsType awsSnsType)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        return awsSnsType switch
        {
            AwsSnsType.SqsFifo => await PublishSqsFifoAsync(
                parameter.DefaultMessage,
                parameter.TopicArn,
                parameter.MessageGroupId,
                parameter.Subject),

            AwsSnsType.Http or AwsSnsType.Https or
            AwsSnsType.email or AwsSnsType.emailJson or
            AwsSnsType.Sqs => await PublishStandardAsync(
                parameter.Message,
                parameter.TopicArn,
                parameter.Subject),

            AwsSnsType.Sms => await PublishSmsAsync(
                parameter.DefaultMessage,
                parameter.PhoneNumber,
                parameter.Subject),

            _ => false,
        };
    }

    /// <summary>
    /// Publishes a message to an AWS SNS FIFO (First-In-First-Out) topic.
    /// </summary>
    /// <param name="message">The message content to be published.</param>
    /// <param name="topicArn">The ARN (Amazon Resource Name) of the SNS topic.</param>
    /// <param name="messageGroupId">A unique identifier for the message group.</param>
    /// <param name="subject">An optional subject for the message.</param>
    /// <returns>A boolean indicating whether the message was successfully published.</returns>
    private async Task<bool> PublishSqsFifoAsync(
        string message,
        string topicArn,
        Guid messageGroupId,
        string? subject = null)
    {
        subject ??= SUBJECT_EMPTY;

        try
        {
            ArgumentNullException.ThrowIfNull(topicArn);
            ArgumentNullException.ThrowIfNull(messageGroupId);

            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = message,
                Subject = subject,
                MessageGroupId = messageGroupId.ToString() ?? null,
            };

            var response = await _snsClient.PublishAsync(request);

            _logger.LogInformation($"Message published with ID: {response.MessageId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// Publishes a message to a standard AWS SNS topic (e.g., HTTP, HTTPS, Email, or standard SQS).
    /// </summary>
    /// <param name="message">The structured message object to be published.</param>
    /// <param name="topicArn">The ARN (Amazon Resource Name) \of the SNS topic.</param>
    /// <param name="subject">An optional subject for the message.</param>
    /// <returns>A boolean indicating whether the message was successfully published.</returns>
    private async Task<bool> PublishStandardAsync(
    Message message,
    string topicArn,
    string? subject = null)
    {
        subject ??= SUBJECT_EMPTY;

        try
        {
            ArgumentNullException.ThrowIfNull(topicArn);
            ArgumentNullException.ThrowIfNull(message);

            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = JsonConvert.SerializeObject(message),
                Subject = subject,
                MessageStructure = JSON_MESSAGE_FORMAT
            };

            var response = await _snsClient.PublishAsync(request);

            _logger.LogInformation($"Message published with ID: {response.MessageId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// Publishes a message directly to a phone number using AWS SNS.
    /// </summary>
    /// <param name="message">The message content to be sent via SMS.</param>
    /// <param name="phoneNumber">The recipient's phone number.</param>
    /// <param name="subject">An optional subject for the message.</param>
    /// <returns>A boolean indicating whether the message was successfully published.</returns>
    private async Task<bool> PublishSmsAsync(
        string message,
        string phoneNumber,
        string? subject = null)
    {
        subject ??= SUBJECT_EMPTY;

        try
        {
            ArgumentNullException.ThrowIfNull(phoneNumber);

            var request = new PublishRequest
            {
                Message = message,
                Subject = subject,
                PhoneNumber = phoneNumber
            };

            var response = await _snsClient.PublishAsync(request);

            _logger.LogInformation($"Message published with ID: {response.MessageId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred: {ex.Message}", ex);
            return false;
        }
    }
}