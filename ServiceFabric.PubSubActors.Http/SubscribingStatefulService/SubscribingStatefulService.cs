using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.PubSubActors.SubscriberServices;
using ServiceFabric.PubSubActors.Helpers;
using Microsoft.ServiceFabric.Data;
using Common.DataContracts;
using ServiceFabric.PubSubActors.Interfaces;

namespace SubscribingStatefulService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SubscribingStatefulService : StatefulService, ISubscriberService
    {
        private readonly ISubscriberServiceHelper _subscriberServiceHelper;

        private readonly StatefulServiceContext _serviceContext;


        public SubscribingStatefulService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
            _subscriberServiceHelper = new SubscriberServiceHelper(new BrokerServiceLocator());
            _serviceContext = serviceContext;
        }

        public SubscribingStatefulService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica) : base(serviceContext, reliableStateManagerReplica)
        {
            _subscriberServiceHelper = new SubscriberServiceHelper(new BrokerServiceLocator());
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            yield return new ServiceReplicaListener(p => new SubscriberCommunicationListener(this, p), "StatefulSubscriberCommunicationListener");
        }

        protected override async Task OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken)
        {
            await TryRegisterAsync();
        }

        private async Task TryRegisterAsync()
        {
            int retries = 0;
            const int maxRetries = 10;
            Thread.Yield();
            while (true)
            {
                try
                {
                    await RegisterAsync();
                    ServiceEventSource.Current.ServiceMessage(_serviceContext, $"Registered Service:'{nameof(SubscribingStatefulService)}' Replica:'{Context.ReplicaId}' as Subscriber.");
                    break;
                }
                catch (Exception ex)
                {
                    if (retries++ < maxRetries)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                        continue;
                    }
                    ServiceEventSource.Current.ServiceMessage(_serviceContext, $"Failed to register Service:'{nameof(SubscribingStatefulService)}' Replica:'{Context.ReplicaId}' as Subscriber. Error:'{ex}'");
                    break;
                }
            }
        }

        public async Task RegisterAsync()
        {
            await this.RegisterMessageTypeAsync(typeof(PublishedMessageOne));
            //await this.RegisterMessageTypeWithBrokerServiceAsync(typeof(PublishedMessageTwo));
            await _subscriberServiceHelper.RegisterMessageTypeAsync(this, typeof(PublishedMessageTwo));
        }

        public async Task UnregisterAsync()
        {
            await this.UnregisterMessageTypeAsync(typeof(PublishedMessageOne), true);
            //await this.UnregisterMessageTypeWithBrokerServiceAsync(typeof(PublishedMessageTwo), true);
            await _subscriberServiceHelper.UnregisterMessageTypeAsync(this, typeof(PublishedMessageTwo), true);
        }

        public Task ReceiveMessageAsync(MessageWrapper message)
        {
            var payload = this.Deserialize<PublishedMessageOne>(message);
            ServiceEventSource.Current.ServiceMessage(_serviceContext, $"Received message: {payload.Content}");
            //TODO: handle message
            return Task.FromResult(true);
        }
    }
}
