using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using PublishingActor.Interfaces;
using ServiceFabric.PubSubActors.Helpers;
using ServiceFabric.PubSubActors.PublisherActors;
using Common.DataContracts;

namespace PublishingActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class PublishingActor : Actor, IPublishingActor
    {
        private readonly IPublisherActorHelper _publisherActorHelper;

        public PublishingActor(ActorService actorService, ActorId actorId, IPublisherActorHelper publisherActorHelper = null)
            : base(actorService, actorId)
        {
            _publisherActorHelper = publisherActorHelper ?? new PublisherActorHelper(new BrokerServiceLocator());
        }

        async Task<string> IPublishingActor.PublishMessageOneAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Publishing Message");
            //using broker actor
            await this.PublishMessageAsync(new PublishedMessageOne { Content = "Hello PubSub World, from Actor, using Broker Actor!" });

            return "Message published to broker actor";
        }

        async Task<string> IPublishingActor.PublishMessageTwoAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Publishing Message");
            //using broker service
            //wrong type:
            await _publisherActorHelper.PublishMessageAsync(this, new PublishedMessageOne { Content = "If you see this, something is wrong!" });
            //right type:
            await _publisherActorHelper.PublishMessageAsync(this, new PublishedMessageTwo { Content = "Hello PubSub World, from Actor, using Broker Service!" });

            return "Message published to broker service";
        }
    }
}
