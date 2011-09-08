using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;


namespace ALFA_Client
{
    using ClientServiceReference;

    class ClientServiceCallback : IClientServiceCallback
    {

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public void AlertAboutControllerNotResponsible(string portName, byte controllerNumber)
        {
            _logger.Debug("controller not responsible port name {0}, controller number {1}");
        }

        public void AlertAboutControllerBeganRespond(string portName, byte controllerNumber)
        {
            throw new NotImplementedException();
        }

        public void AlertComPortNotResponsible(string portName)
        {
            throw new NotImplementedException();
        }

        public void AlertComPortBeganRespond(string portName)
        {
            throw new NotImplementedException();
        }

        public void AlertGerkon(long roomId)
        {
            _logger.Debug("gercon port name {0}, controller number {1}");
            RoomCollection.UpdateGerkon(roomId);
        }

        public void AlertUnsetKey(string portName, byte controllerNumber)
        {
            _logger.Debug("unset key port name {0}, controller number {1}");
        }

        public void AlertChangeRoomsOnTheFloor(string portName)
        {
            _logger.Debug("change rooms port name {0}");
        }

        public void AlertChangeFloors(string portName)
        {
            _logger.Debug("change floors port name {0}");
        }
    }
}
