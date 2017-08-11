using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SubscribingActor.Interfaces;
using ServiceFabric.PubSubActors.Helpers;
using Common.DataContracts;
using ServiceFabric.PubSubActors.SubscriberActors;
using ServiceFabric.PubSubActors.Interfaces;

namespace SubscribingActor
{
    /// <remarks>
	/// Each ActorID maps to an instance of this class.
	/// The ISubscribingActor interface (in a separate DLL that client code can
	/// reference) defines the operations exposed by SubscribingActor objects.
	/// </remarks>
	[ActorService(Name = nameof(ISubscribingActor))]
    [StatePersistence(StatePersistence.None)]
    internal class SubscribingActor : Actor, ISubscribingActor
    {
        private const string WellKnownRelayBrokerId = "WellKnownRelayBroker";
        private readonly ISubscriberActorHelper _subscriberActorHelper;

        public SubscribingActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
            _subscriberActorHelper = new SubscriberActorHelper(new BrokerServiceLocator());
        }

        public Task RegisterAsync()
        {
            return this.RegisterMessageTypeAsync(typeof(PublishedMessageOne)); //register as subscriber for this type of messages
        }

        public Task UnregisterAsync()
        {
            return this.UnregisterMessageTypeAsync(typeof(PublishedMessageOne), true); //unregister as subscriber for this type of messages
        }

        public Task RegisterWithRelayAsync()
        {
            //register as subscriber for this type of messages at the relay broker
            //using the default Broker for the message type as source for the relay broker
            return this.RegisterMessageTypeWithRelayBrokerAsync(typeof(PublishedMessageOne), new ActorId(WellKnownRelayBrokerId), null);
        }

        public Task UnregisterWithRelayAsync()
        {
            //unregister as subscriber for this type of messages at the relay broker
            return this.UnregisterMessageTypeWithRelayBrokerAsync(typeof(PublishedMessageOne), new ActorId(WellKnownRelayBrokerId), null, true);
        }



        public Task RegisterWithBrokerServiceAsync()
        {
            //return this.RegisterMessageTypeWithBrokerServiceAsync(typeof(PublishedMessageTwo));
            return _subscriberActorHelper.RegisterMessageTypeAsync(this, typeof(PublishedMessageTwo));

        }

        public Task UnregisterWithBrokerServiceAsync()
        {
            //return this.UnregisterMessageTypeWithBrokerServiceAsync(typeof(PublishedMessageTwo), true);
            return _subscriberActorHelper.UnregisterMessageTypeAsync(this, typeof(PublishedMessageTwo), true);
        }

        public Task ReceiveMessageAsync(MessageWrapper message)
        {
            var payload = this.Deserialize<PublishedMessageOne>(message);
            ActorEventSource.Current.ActorMessage(this, $"Received message: {payload.Content}");
            //TODO: handle message
            return Task.FromResult(true);
        }
    }
}
