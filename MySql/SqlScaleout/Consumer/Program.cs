using System;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Logging;

namespace Consumer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                adapter.Handle<Job>(async job =>
                {
                    Console.WriteLine("Processing job {0}", job.JobNumber);

                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                });

                const string connectionString = "server=localhost;database=rebus;user=mysql;password=mysql;maximum pool size=30;allow user variables=true;";
                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseMySql(new MySqlTransportOptions(connectionString), "consumer"))
                    .Options(o =>
                    {
                        o.SetNumberOfWorkers(1);
                        o.SetMaxParallelism(5);
                    })
                    .Start();

                Console.WriteLine("Consumer listening - press ENTER to quit");
                Console.ReadLine();
            }
        }
    }
}
