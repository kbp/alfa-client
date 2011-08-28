using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using ALFA_Client.ClientServiceReference;

namespace ALFA_Client
{
    class ServiceClient
    {
        private ClientServiceClient _client;


        private ServiceClient()
        {
            
        }

        private static ServiceClient _instance = null;
        public static ServiceClient GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ServiceClient();
                InstanceContext instanceContext = new InstanceContext(new ClientServiceCallback());
                _instance._client = new ClientServiceClient(instanceContext);
            }

            return _instance;
        }

        public ClientServiceClient GetClientServiceClient()
        {
            return _client;
        }
    }
}
