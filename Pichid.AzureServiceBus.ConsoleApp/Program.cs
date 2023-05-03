using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

var credentialOptions = new VisualStudioCredentialOptions
{
    TenantId = "3b4746c8-cc8f-4413-8e2c-5dad5c063418"
};
TokenCredential tokenCredential = new VisualStudioCredential(credentialOptions);

var clientOptions = new ServiceBusClientOptions
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
ServiceBusClient client = new ServiceBusClient(
    "pichidazureservicebus.servicebus.windows.net",
    tokenCredential,
    clientOptions);

ServiceBusSender sender = client.CreateSender("testqueue");

using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

if (!messageBatch.TryAddMessage(new ServiceBusMessage("Test Message")))
{
    throw new Exception("The message is too large to fit in the batch.");
}

try
{
    await sender.SendMessagesAsync(messageBatch);

    Console.WriteLine("A batch of message has been published to the queue.");
}
catch (ServiceBusException exception)
{
    Console.WriteLine($"ServiceBusException: {exception.Message}");
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();
