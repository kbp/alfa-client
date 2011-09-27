using System;
using System.ComponentModel;
using System.ServiceModel;
using ALFA_Client.ClientServiceReference;
using NLog;

namespace ALFA_Client
{
    class ServiceClient : INotifyPropertyChanged
    {
        private ClientServiceClient _client;
        static readonly object Lock = new object();

        private ServiceClient()
        {
            
        }

        private static ServiceClient _instance = null;
        public static ServiceClient GetInstance()
        {
            lock (Lock)
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
        }

        public ClientServiceClient GetClientServiceClient()
        {
            return _client;
        }

        private bool _serverOnline = false;
        public bool ServerOnline
        {
            get { return _serverOnline; }
            set
            {
                _serverOnline = value;
                NotifyPropertyChanged("ServerOnline");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
