﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.Routing.TypeBased;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                const string connectionString = "server=localhost;database=rebus;user=mysql;password=mysql;maximum pool size=30;allow user variables=true;";
                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseMySqlAsOneWayClient(new MySqlTransportOptions(connectionString)))
                    .Routing(r => r.TypeBased().MapAssemblyOf<Job>("consumer"))
                    .Start();

                var keepRunning = true;

                while (keepRunning)
                {
                    Console.WriteLine(@"a) Send 10 jobs
b) Send 100 jobs
c) Send 1000 jobs

q) Quit");
                    var key = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (key)
                    {
                        case 'a':
                            Send(10, adapter.Bus);
                            break;
                        case 'b':
                            Send(100, adapter.Bus);
                            break;
                        case 'c':
                            Send(1000, adapter.Bus);
                            break;
                        case 'q':
                            Console.WriteLine("Quitting");
                            keepRunning = false;
                            break;
                    }
                }
            }
        }

        static void Send(int numberOfJobs, IBus bus)
        {
            Console.WriteLine("Publishing {0} jobs", numberOfJobs);

            var sendTasks = Enumerable.Range(0, numberOfJobs)
                .Select(i => new Job(i))
                .Select(job => bus.Send(job))
                .ToArray();

            Task.WaitAll(sendTasks);
        }
    }
}
