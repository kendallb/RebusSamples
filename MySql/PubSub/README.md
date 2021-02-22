## PubSub

This sample demonstrates how a publisher can publish messages to two subscribers, all using MySQL as the transport.

In order to run the sample, you need to have access to a MySQL server, which is assumed 
to be running locally as the default instance, containing a database called 'rebus' with access 
to that table with the user 'mysql' and password 'mysql'. A table, 'Messages', will automatically 
be created in the database.

Naturally, you're free to edit the connection string in the publishers and subscribers. 

The type-based router is used (which means that you subscribe/unsubscribe to specific message types) 
along with a local subscription storage (which means that the publisher stores subscriptions locally, 
and subscribers must subscribe by sending a `SubscribeRequest` to the publisher - this is what happens
underneath the covers when they `await bus.Subscribe`).

Contrary to [centralized pub/sub samples](/PubSubCentralized), endpoint mappings are required at
the subscribers' end, because they need to know where to send their `SubscribeRequest`s. This has
the advantage that the publisher can store subscriptions locally in whichever way it may prefer.

Make sure you start the publisher first, because it will need to create its own input queue. 

After having started the publisher once, you may run the publisher and the two subscribers in any
order, and still all events will be delivered.