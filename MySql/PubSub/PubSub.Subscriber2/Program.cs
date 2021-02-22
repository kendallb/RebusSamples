using System;
using System.Threading.Tasks;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
using Rebus.Routing.TypeBased;
#pragma warning disable 1998

namespace PubSub.Subscriber2
{
    class Program
    {
        static void Main()
        {
            using (var activator = new BuiltinHandlerActivator())
            {
                activator.Register(() => new Handler());

                const string connectionString = "server=localhost;database=rebus;user=mysql;password=mysql;maximum pool size=30;allow user variables=true;";
                Configure.With(activator)
                         .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                         .Transport(t => t.UseMySql(new MySqlTransportOptions(connectionString), "subscriber2"))
                         .Routing(r => r.TypeBased().MapAssemblyOf<StringMessage>("publisher"))
                         .Start();

                activator.Bus.Subscribe<StringMessage>().Wait();

                Console.WriteLine("This is Subscriber 2");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    class Handler : IHandleMessages<StringMessage>
    {
        public async Task Handle(StringMessage message)
        {
            Console.WriteLine("Got string: {0}", message.Text);
        }
    }
}
