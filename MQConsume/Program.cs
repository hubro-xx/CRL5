using CRL.Core.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQConsume
{
    class Program
    {
        static string mqUser = "henanhaiwang";
        static string mqPass = "henanhaiwang";
        static string mqHost = "47.105.149.240";
        static void Main(string[] args)
        {
            var client = new TopicRabbitMQ<MessageObj>(mqHost, mqUser, mqPass, "exchange1");
            //client.SetRoutingKey("tp1");
            //var client = new FanoutRabbitMQ<MessageObj>(mqHost, mqUser, mqPass, "exchange2");
            //var client = new DirectRabbitMQ<MessageObj>(mqHost, mqUser, mqPass, "queue2");
            client.BeginReceive(obj =>
            {
                Console.WriteLine($"receive " + obj.name);
            }, "tp1");
            Console.ReadLine();
        }
    }
    public class MessageObj
    {
        public string name
        {
            get; set;
        }
    }
}
