using System;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;

namespace PubSub.Publisher
{
    class Program
    {
        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                const string connectionString = "server=localhost;database=rebus;user=mysql;password=mysql;maximum pool size=30;allow user variables=true;";
                Configure.With(activator)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMySql(new MySqlTransportOptions(connectionString), "publisher"))
                    .Subscriptions(s => s.StoreInMySql(connectionString, "Subscriptions", isCentralized: true))
                    .Start();

                var startupTime = DateTime.Now;

                while (true)
                {
                    Console.WriteLine(@"a) Publish string
b) Publish DateTime
c) Publish TimeSpan
q) Quit");

                    var keyChar = char.ToLower(Console.ReadKey(true).KeyChar);
                    var bus = activator.Bus.Advanced.SyncBus;

                    switch (keyChar)
                    {
                        case 'a':
                            bus.Publish(new StringMessage("Hello there, this is a string message from a publisher!"));
                            break;

                        case 'b':
                            bus.Publish(new DateTimeMessage(DateTime.Now));
                            break;

                        case 'c':
                            bus.Publish(new TimeSpanMessage(DateTime.Now - startupTime));
                            break;

                        case 'q':
                            goto consideredHarmful;

                        default:
                            Console.WriteLine($"There's no option '{keyChar}'");
                            break;
                    }
                }

            consideredHarmful: ;
                Console.WriteLine("Quitting!");
            }
        }
    }
}
