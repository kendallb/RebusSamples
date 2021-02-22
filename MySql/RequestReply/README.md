## RequestReply

This sample demonstrates how a producer can produce jobs which any number of parallel consumers can consume.

In order to run the sample, you need to have access to a MySQL server, which is assumed
to be running locally as the default instance, containing a database called 'rebus' with access
to that table with the user 'mysql' and password 'mysql'. A table, 'Messages', will automatically
be created in the database.