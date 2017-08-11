using Common.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using PublishingActor.Interfaces;
using SubscribingActor.Interfaces;
using SubscribingStatelessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{
    public static class extension
    {
        public static ISubscribingStatelessService GetSubStatelessProxy(this Controller ctrl)
        {
            ISubscribingStatelessService proxy = null;

            while (proxy == null)
            {
                try
                {
                    proxy = ServiceProxy.Create<ISubscribingStatelessService>(new Uri("fabric:/MyServiceFabricApp/SubscribingStatelessService"),
                        listenerName: "StatelessFabricTransportServiceRemotingListener");
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
            return proxy;
        }

        public static IPublishingStatelessService GetPublishingService(this Controller ctrl, Uri serviceName)
        {
            IPublishingStatelessService pubService = null;

            while (pubService == null)
            {
                try
                {
                    pubService = ServiceProxy.Create<IPublishingStatelessService>(serviceName);
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
            return pubService;
        }

        public static IPublishingActor GetPublishingActor(this Controller ctrl, string applicationName)
        {
            IPublishingActor pubActor = null;
            var actorId = new ActorId("PubActor");

            while (pubActor == null)
            {
                try
                {
                    pubActor = ActorProxy.Create<IPublishingActor>(actorId, applicationName);
                }
                catch
                {
                    Thread.Sleep(200);
                }
            }
            return pubActor;
        }

        public static void RegisterSubscribers(string applicationName)
        {
            for (int i = 0; i < 4; i++)
            {
                var actorId = new ActorId("SubActor" + i.ToString("0000"));

                ISubscribingActor subActor = null;
                while (subActor == null)
                {
                    try
                    {
                        subActor = ActorProxy.Create<ISubscribingActor>(actorId, new Uri("fabric:/SF.PubSubActors.Http/ISubscribingActor"));

                        if (i % 3 == 0)
                        {
                            subActor.RegisterAsync().GetAwaiter().GetResult();
                        }
                        else if (i % 3 == 1)
                        {
                            subActor.RegisterWithRelayAsync().GetAwaiter().GetResult();
                        }
                        else
                        {
                            subActor.RegisterWithBrokerServiceAsync().GetAwaiter().GetResult();
                        }
                    }
                    catch
                    {
                        subActor = null;
                        Thread.Sleep(200);
                    }
                }
            }
        }
    }
}
