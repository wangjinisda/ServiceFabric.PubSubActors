using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using PubSubActor.Interfaces;
using ServiceFabric.PubSubActors.Interfaces;

namespace PubSubActor
{
    [ActorService(Name = nameof(IBrokerActor))]
    internal class PubSubActor : ServiceFabric.PubSubActors.BrokerActor, IPubSubActor
    {
        public PubSubActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            ActorEventSourceMessageCallback = message => ActorEventSource.Current.ActorMessage(this, message);
        }
    }
}
