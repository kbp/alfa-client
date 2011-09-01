using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using ALFA_Client.ClientServiceReference;
using NLog;

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
            Logger logger = LogManager.GetCurrentClassLogger();

            if (_instance == null)
            {
                _instance = new ServiceClient();
                logger.Info("service initialize");
                InstanceContext instanceContext = new InstanceContext(new ClientServiceCallback());
                _instance._client = new ClientServiceClient(instanceContext);
            }

            
            logger.Info("service instance");

            return _instance;
        }

        public ClientServiceClient GetClientServiceClient()
        {
            return _client;
        }
    }
}
