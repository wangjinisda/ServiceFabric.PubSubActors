using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common.DataContracts
{
    [ServiceContract]
    public interface IPublishingStatelessService : IService
    {
        [OperationContract]
        Task<string> PublishMessageOneAsync();

        [OperationContract]
        Task<string> PublishMessageTwoAsync();
    }
}
