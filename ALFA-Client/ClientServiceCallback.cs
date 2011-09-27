using System;
using ALFA_Client.Models;
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
            LogCollection.GetInstance().Info("контроллер №" + controllerNumber + " перестал отвечать");
            RoomCollection.RemoveByControllerNumber(controllerNumber);
        }

        public void AlertAboutControllerBeganRespond(string portName, byte controllerNumber)
        {
            _logger.Debug("controller began respond port name {0}, controller number {1}");
            LogCollection.GetInstance().Info("контроллер №" + controllerNumber + " начал отвечать");
            RoomCollection.AddByControllerNumber(portName, controllerNumber);
        }

        public void AlertComPortNotResponsible(string portName)
        {
        }

        public void AlertComPortBeganRespond(string portName)
        {
        }

        public void AlertGerkon(long roomId)
        {
            _logger.Debug("gercon port name {0}, controller number {1}");
            LogCollection.GetInstance().Info("комната с идентификатором " + roomId + " была открыта");
            RoomCollection.UpdateGerkon(roomId);
        }

        public void AlertUnsetKey(string portName, byte controllerNumber)
        {
            RoomCollection.UpdateKeys(portName, controllerNumber);
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
