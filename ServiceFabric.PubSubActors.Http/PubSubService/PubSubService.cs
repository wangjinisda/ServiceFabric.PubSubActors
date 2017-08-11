using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting;
using PubSubService.Service;
using ServiceFabric.PubSubActors;

namespace PubSubService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class PubSubService : BrokerService
    {
        public PubSubService(StatefulServiceContext context)
            : base(context)
        {
            ServiceEventSourceMessageCallback = message => ServiceEventSource.Current.ServiceMessage(context, message);
        }
    }
}
