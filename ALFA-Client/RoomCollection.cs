using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ALFA_Client
{
    using Entities;

    public class RoomsEnter : INotifyPropertyChanged
    {
        public RoomsEnter(int roomId, int room, bool guardOn, bool lightOn, byte controllerId, bool online)
        {
            _room = room;
            _roomId = roomId;
            _guardOn = guardOn;
            _lightOn = lightOn;
            _controllerId = controllerId;
            _online = online;
            _alarm = false;
            _keys = new KeysCollection();
            _keys.Fill(roomId);
        }

        private int _room;
        private int _roomId;
        private bool _guardOn;
        private bool _lightOn;
        private bool _online;
        private byte _controllerId;
        private bool _alarm = false;

        public int Room
        {
            get { return _room; }
            set { _room = value; }
        }
        
        public bool Alarm
        {
            get { return _alarm; }
            set
            {
                _alarm = value;
                NotifyPropertyChanged("Alarm");
            }
        }

        public bool Online
        {
            get { return _online; }
            set { _online = value;}
        }

        public int RoomId
        {
            get { return _roomId; }
            set { _roomId = value; }
        }        
        
        public bool GuardOn
        {
            get { return _guardOn; }
            set
            {
                AlfaEntities alfaEntities = new AlfaEntities();

                var room = (from rooms in alfaEntities.Rooms.Include("Floors")
                            where rooms.RoomId == _roomId
                            select rooms).FirstOrDefault();
                if (room != null)
                {
                    if (ServiceClient.GetInstance().GetClientServiceClient().SetRoomToProtect(room.Floors.ComPort, _controllerId, value))
                    {
                        _guardOn = value;
                    }
                }
            }
        }

        public bool LightOn
        {
            get { return _lightOn; }
            set 
            { 
                //todo сдлать обработку ошибок, точней обработку FALSE значений которые возвращает сервер
                AlfaEntities alfaEntities = new AlfaEntities();

                var room = (from rooms in alfaEntities.Rooms.Include("Floors")
                            where rooms.RoomId == _roomId
                            select rooms).FirstOrDefault();
                if (room != null)
                {
                    if (ServiceClient.GetInstance().GetClientServiceClient().SetLight(room.Floors.ComPort, _controllerId, value))
                    {
                        _lightOn = value;
                    }
                }
            }
        }

        public byte ControllerId
        {
            get { return _controllerId; }
            set { _controllerId = value; }
        }

        private KeysCollection _keys;
        public KeysCollection Keys
        {
            get { return _keys; }
            set
            {
                NotifyPropertyChanged("Keys");
                _keys = value;
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

     public class RoomCollection : ObservableCollection<RoomsEnter>, INotifyPropertyChanged
     {
         public RoomCollection()
         {
             _roomCollection = this;
         }

         private static RoomCollection _roomCollection;

         public static void UpdateGerkon(long roomId)
         {
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
                 if (roomsEnter.RoomId == roomId)
                 {
                     roomsEnter.Alarm = true;
                 }
             }
         }

         public void Fill(int floorId)
         {
             _roomsCount = 0;
             AlfaEntities alfaEntities = new AlfaEntities();
             IEnumerable<Rooms> rooms = from roomse in alfaEntities.Rooms
                                        where roomse.FloorId == floorId
                                        select roomse;

             //todo можно убрать вместе с ифами когда будут выставлены дефалтные значения в базе
             foreach (var room in rooms)
             {
                 _roomsCount++;
                 if (room.IsProtected == null)
                 {
                     room.IsProtected = false;
                 }

                 if (room.LightOn == null)
                 {
                     room.LightOn = true;
                 }

                 if (room.RoomNumber != null)
                 {
                     if (room.OnLine != null)
                         this.Add(new RoomsEnter(room.RoomId, (int) room.RoomNumber, (bool) room.IsProtected,
                                                 (bool)room.LightOn, (byte)room.ConrollerId, (bool)room.OnLine));

                 }
             }

             Counter = "" + this.Count + "/" + _roomsCount;
             Counter = "" + this.Count + "/" + _roomsCount;
             //todo wtf
         }

         public static void RemoveByControllerNumber(byte controllerNumber)
         {
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
                 if (roomsEnter.ControllerId == controllerNumber)
                 {
                     _roomCollection.Remove(roomsEnter);
                 }
             }
             _roomCollection.Counter = "" + _roomCollection.Count + "/" + _roomCollection._roomsCount;
         }

         public static void AddByControllerNumber(string portName, byte controllerNumber)
         {
             AlfaEntities alfaEntities = new AlfaEntities();

             Rooms room = (from roomse in alfaEntities.Rooms.Include("Floors")
                           where roomse.Floors.ComPort == portName && roomse.ConrollerId == controllerNumber
                           select roomse).FirstOrDefault();

             if (room.IsProtected == null)
             {
                 room.IsProtected = false;
             }

             if (room.LightOn == null)
             {
                 room.LightOn = true;
             }

             if (room != null)
             {
                 _roomCollection.Add(new RoomsEnter(room.RoomId, (int)room.RoomNumber, (bool) room.IsProtected, (bool) room.LightOn, 
                     (byte) room.ConrollerId, (bool)room.OnLine));
             }

             _roomCollection.Counter = "" + _roomCollection.Count + "/" + _roomCollection._roomsCount;
         }

         public static void UpdateKeys(string portName, byte controllerNumber)
         {
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
                 if (roomsEnter.ControllerId == controllerNumber)
                 {
                     roomsEnter.Keys.Fill(roomsEnter.RoomId);
                     return;
                 }
             }
         }

         private int _roomsCount;

         private string _counter;
         public string Counter
         {
             get { return _counter; }
             set
             {
                 NotifyPropertyChanged("Counter");
                 _counter = value;
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
