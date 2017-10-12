using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using UserAgent.Interfaces;
using Nethereum;

namespace UserAgent
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserAgent : Actor, IUserAgent
    {
        private DelegatedIdentity identity = new DelegatedIdentity();
        
        public UserAgent(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
        
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            
            this.StateManager.TryAddStateAsync("reservation", "all the reservation results");

            return identity.Initialize();
        }

        public Task<string> GetTransportAsync(string schedule)
        {
            return this.StateManager.GetStateAsync<string>("reservation");
        }
    }
}
