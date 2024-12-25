# AWS SNS

A .NET Console Application designed to demonstrate interaction with AWS Simple Notification Service (SNS). The application provides an `AwsSns` class that encapsulates common SNS operations, such as publishing messages to FIFO topics, standard topics, and SMS.

## What are the functions?

The main functions of the `AwsSns` class are:

1. **Publish to FIFO SNS Topic**  
   Publishes a message to an AWS SNS FIFO topic and supports attributes like `MessageGroupId`, `TopicArn`, and `Subject`.

2. **Publish to Standard SNS Topic**  
   Publishes a structured JSON message to a standard SNS topic and supports multiple protocols like `HTTP`, `HTTPS`, `Email`, `EmailJson` and `SQS`.

3. **Publish SMS Message**  
   Sends a direct SMS message using AWS SNS and includes message content and an optional subject.

## AWS SNS Service
AWS SNS is a fully managed pub/sub messaging service for sending messages to distributed systems, microservices, or mobile devices. This application interacts with SNS using the AWS SDK.

### How to use the application

#### 1. Update AWS Configuration
Modify the constants in the code for your specific SNS setup:
- **AWS_REGION**: Specify the AWS region where your SNS is hosted (e.g., `us-east-1`).
- **AWS_ACCOUNT_ID**: Replace with your AWS account ID.
- **AWS_SNS_TOPIC**: Replace with your SNS topic name.

Example:
```csharp
const string AWS_REGION = "us-east-1";
const string AWS_ACCOUNT_ID = "123456789012";
const string AWS_QUEUE_NAME = "topic-test.fifo";
```

#### 2. Set up Logger
The application uses a console logger to provide feedback on operations. No additional configuration is needed for basic logging.

#### 3. Publish to a FIFO SNS Topic
The `PublishAsync` method sends a message to an SNS FIFO topic. It supports `MessageGroupId` to maintain order.

#### Example usage:
```csharp
await awsSns.PublishAsync(
    new Parameter
    {
        TopicArn = TOPIC_ARN,
        DefaultMessage = "Hello World",
        MessageGroupId = Guid.NewGuid()
    },
    AwsSnsType.SqsFifo
);
```
#### 4. Publish to a Standard SNS Topic
The `PublishAsync` method also supports publishing to standard SNS topics, including support for multiple protocols (SQS, HTTP, HTTPS, Email and EmailJson).

#### Example usage:
```csharp
await awsSns.PublishAsync(
    new Parameter
    {
        TopicArn = TOPIC_ARN,
        Message = new Message
        {
            Http = "HTTP Hello World"
        }
    },
    AwsSnsType.Http
);
```

#### 5. Send an SMS Message
Directly send an SMS message using SNS with the `PublishAsync` method.

#### Example usage:
```csharp
await awsSns.PublishAsync(
    new Parameter
    {
        DefaultMessage = "This is an SMS Hello World notification.",
        PhoneNumber = "+1234567890"
    },
    AwsSnsType.Sms
);
```

## Points to Highlight
- **FIFO Topic Support**: Includes features like `MessageGroupId` for message ordering and deduplication.
- **Multi-Protocol Support**: Seamlessly publishes messages to various endpoints (HTTP, HTTPS, Email, EmailJson, SMS, SQS, FIFO SQS).
- **Logging**: Logs information on successful operations and errors for better traceability.

## Before Running the Application
1. Ensure you have valid AWS credentials configured on your system or in the AWS SDK for .NET.
2. Update the constants in the code to match your SNS setup.
3. Ensure your SNS topic (e.g., `topic-test.fifo`) is created and ready to use with all the correct subscriptions.
4. Verify that the IAM role or user has sufficient permissions for SNS operations.

## About
This project is developed using:

- **.NET 8**
- **AWS SDK for .NET**
- **Newtonsoft.Json** for JSON serialization
- **Microsoft.Extensions.Logging** for logging

It provides a foundational class to handle SNS operations with a focus on simplicity and extensibility.