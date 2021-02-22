using System;
using Consumer.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Routing.TypeBased;

namespace Producer
{
    class Program
    {
        const string ConnectionString = "server=localhost;database=rebus;user=mysql;password=mysql;maximum pool size=30;allow user variables=true;";

        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                adapter.Handle<Reply>(async reply =>
                {
                    await Console.Out.WriteLineAsync($"Got reply '{reply.KeyChar}' (from OS process {reply.OsProcessId})");
                });

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMySql(new MySqlTransportOptions(ConnectionString), "producer"))
                    .Routing(r => r.TypeBased().MapAssemblyOf<Job>("consumer"))
                    .Start();

                Console.WriteLine("Press Q to quit or any other key to produce a job");
                while (true)
                {
                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (keyChar)
                    {
                        case 'q':
                            goto quit;

                        default:
                            adapter.Bus.Send(new Job(keyChar)).Wait();
                            break;
                    }
                }

            quit:
                Console.WriteLine("Quitting...");
            }
        }
    }
}
