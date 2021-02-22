using System;
using System.Diagnostics;
using Consumer.Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;

namespace Consumer
{
    class Program
    {
        const string ConnectionString = "server=localhost;database=rebus;user=mysql;password=mysql;maximum pool size=30;allow user variables=true;";

        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                adapter.Handle<Job>(async (bus, job) =>
                {
                    var keyChar = job.KeyChar;
                    var processId = Process.GetCurrentProcess().Id;
                    var reply = new Reply(keyChar, processId);

                    await bus.Reply(reply);
                });

                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(minLevel: LogLevel.Warn))
                    .Transport(t => t.UseMySql(new MySqlTransportOptions(ConnectionString), "consumer"))
                    .Start();

                Console.WriteLine("Press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
