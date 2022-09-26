﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json;
using SimpleMessaging;

namespace Sender
{
    class Consumer
    {
        // The consumer has to run in a loop as its acting as a server and responding to messages from clients. If you try to run
        // asynchronously from the client then it is unlikely you would receive and respond before the calling application
        // decided to time you out
        static void Main(string[] args)
        {
          var consumer = new PollingConsumer<Greeting, GreetingResponse>(
               new GreetingHandler(), 
                messageBody => JsonConvert.DeserializeObject<Greeting>(messageBody, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                }),
                (greetingResponse) => JsonConvert.SerializeObject(greetingResponse)
            );

            var tokenSource = new CancellationTokenSource();

            try
            {
                Console.WriteLine("Consumer running, entering loop until signalled");
                Console.WriteLine(" Press [enter] to exit.");
                //has its own thread and will continue until signalled
                var task = consumer.Run(tokenSource.Token);
                while (true)
                {
                    //loop until we get a keyboard interrupt
                    if (Console.KeyAvailable)
                    {
                        //Note: This will deadlock with Console.WriteLine on the task thread unless we have called Writeline first
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Enter)
                        {
                            //signal exit
                            tokenSource.Cancel();
                            //wait for thread to error
                            task.Wait();
                            //in theory we don't get here, because we raise an exception on cancellation, but just in case
                            break;
                        }

                        Task.Delay(3000);  // yield
                    }
                }
            }
            catch (AggregateException ae)
            {
                foreach (var v in ae.InnerExceptions)
                    Console.WriteLine(ae.Message + " " + v.Message);
            }
            finally
            {
                tokenSource.Dispose();
            }
 
        }
    }
}