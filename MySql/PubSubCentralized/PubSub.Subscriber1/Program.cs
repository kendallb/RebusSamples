using System;
using System.Threading.Tasks;
using PubSub.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Logging;
#pragma warning disable 1998

namespace PubSub.Subscriber1
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
                    .Transport(t => t.UseMySql(new MySqlTransportOptions(connectionString), "subscriber1"))
                    .Subscriptions(s => s.StoreInMySql(connectionString, "Subscriptions", isCentralized: true))
                    .Start();

                activator.Bus.Subscribe<StringMessage>().Wait();
                activator.Bus.Subscribe<DateTimeMessage>().Wait();
                activator.Bus.Subscribe<TimeSpanMessage>().Wait();

                Console.WriteLine("This is Subscriber 1");
                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
                Console.WriteLine("Quitting...");
            }
        }
    }

    class Handler : IHandleMessages<StringMessage>, IHandleMessages<DateTimeMessage>, IHandleMessages<TimeSpanMessage>
    {
        public async Task Handle(StringMessage message)
        {
            Console.WriteLine("Got string: {0}", message.Text);
        }

        public async Task Handle(DateTimeMessage message)
        {
            Console.WriteLine("Got DateTime: {0}", message.DateTime);
        }

        public async Task Handle(TimeSpanMessage message)
        {
            Console.WriteLine("Got TimeSpan: {0}", message.TimeSpan);
        }
    }
}
