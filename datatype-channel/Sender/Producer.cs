using System;
using Model;
using Newtonsoft.Json;
using SimpleMessaging;

namespace Sender
{
    class Producer
    {
        static void Main(string[] args)
        {
            using (var channel = new DataTypeChannelProducer<Greeting>((greeting) => JsonConvert.SerializeObject(greeting, Formatting.Indented)))
            {
                var greeting = new Greeting
                {
                    Salutation = "Hello World!"
                };
                channel.Send(greeting);
                Console.WriteLine("Sent message {0}", greeting.Salutation);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}