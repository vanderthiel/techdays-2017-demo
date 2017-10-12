using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserAgent.Interfaces;

namespace DigitalAssistant
{
    class Program
    {
        static void Main(string[] args)
        {
            var actor = ActorProxy.Create<IUserAgent>(ActorId.CreateRandom(), new Uri("fabric:/Transportation/UserAgentActorService"));
            var retval = actor.GetTransportAsync("add schedule here");
            Console.Write(retval.Result);
            Console.ReadLine();
        }
    }
}
