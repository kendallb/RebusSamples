## PubSub Centralized

This sample demonstrates how a publisher can publish messages to two subscribers, using a central
shared subscription storage to manage subscriptions, all using MySQL as the transport.

In order to run the sample, you need to have access to a MySQL server, which is assumed
to be running locally as the default instance, containing a database called 'rebus' with access
to that table with the user 'mysql' and password 'mysql'. A table, 'Messages', will automatically
be created in the database.

Naturally, you're free to edit the connection string in the publishers and subscribers.

Contrary to [the PubSub sample](/PubSub), it is not required that the publisher/subscribers be
started in any particular order, and no endpoint mappings are needed.

You need a local SQL Server database called RebusPubSubCentralized for this demo to work.

The reason a "subscription storage" is needed, is because MSMQ is used as the transport,
and it does not support pub/sub natively.

### Azure Service Bus

For example, if you're using Azure Service Bus, Rebus will leverage its built-in topic-based
routing for publishing events.

Therefore, when using Azure Service Bus the `.Subscriptions(...)` part of the configuration is not
needed, and the configuration can e.g. be changed into this:
```csharp
Configure.With(activator)
    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
    .Transport(t => t.UseAzureServiceBus(connectionString, "subscriber2"))
    .Start();
```

Rebus will then create topics and bindings as needed, so when you
```csharp
await bus.Subscribe<SomeEvent>();
```
a topic named after the assembly, namespace, and type name will be created, and a subscription
beneath it will forward received messages to the caller's input queue (`subscriber2` in this case).

Naturally, when someone then calls
```csharp
await bus.Publish(new SomeEvent(...));
```
the event will be sent to the topic, causing a copy of it to be distributed to each subscriber.

### RabbitMQ

If you're using RabbitMQ, Rebus will work in a similar fashion as when using Azure Service Bus - the
configuration just needs to be changed into something like
```csharp
Configure.With(activator)
    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
    .Transport(t => t.UseRabbitMq(connectionString, "subscriber2"))
    .Start();
```
and then it will work the same way.
