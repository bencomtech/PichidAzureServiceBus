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

ServiceBusProcessor processor = client.CreateProcessor("testqueue");

try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();

    Console.WriteLine("Wait for a minute and then press any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("Stopping the receiver...");

    await processor.StopProcessingAsync();

    Console.WriteLine("Stopped receiving messages");
}
catch (Exception exception)
{
    Console.WriteLine($"ServiceBusException: {exception.Message}");
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}

async Task MessageHandler(ProcessMessageEventArgs arg)
{
    string body = arg.Message.Body.ToString();

    Console.WriteLine($"Received: {body}");

    await arg.CompleteMessageAsync(arg.Message);
}

Task ErrorHandler(ProcessErrorEventArgs arg)
{
    Console.WriteLine(arg.Exception.ToString());

    return Task.CompletedTask;
}
