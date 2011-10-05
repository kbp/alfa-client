using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using ALFA_Client.Models;

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
                    try
                    {
                        if (ServiceClient.GetInstance().ServerOnline)
                        {
                            if (ServiceClient.GetInstance().GetClientServiceClient().SetRoomToProtect(room.Floors.ComPort, _controllerId, value))
                            {
                                _guardOn = value;
                                if (value)
                                {
                                    Counter.GetInstance().RoomsProtected++;
                                    LogCollection.GetInstance().Info("Комната roomId = " + room.RoomId + " поставлена на охрану");
                                }
                                else
                                {
                                    Counter.GetInstance().RoomsProtected--;
                                    LogCollection.GetInstance().Info("Комната roomId = " + room.RoomId + " снята с охраны");
                                }
                                

                            }
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.ToString());
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
                    try
                    {
                        if (ServiceClient.GetInstance().ServerOnline)
                        {
                            if (ServiceClient.GetInstance().GetClientServiceClient().SetLight(room.Floors.ComPort, _controllerId, value))
                            {
                                _lightOn = value;
                                if (value)
                                {
                                    Counter.GetInstance().RoomsLightOn++;
                                    LogCollection.GetInstance().Info("В комнате roomId = " + room.RoomId + " включен свет");
                                }
                                else
                                {
                                    Counter.GetInstance().RoomsLightOn--;
                                    LogCollection.GetInstance().Info("В комнате roomId = " + room.RoomId + " выключен свет");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
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

     public class RoomCollection : MTObservableCollection<RoomsEnter>
     {
         public RoomCollection()
         {
             _roomCollection = this;
         }

         private static RoomCollection _roomCollection;

         private static Thread _stopAlarm;
         private static void SetAlertFalse(object index)
         {
             Thread.Sleep(3000);
             _roomCollection[(int)index].Alarm = false;
         }

         public static void UpdateGerkon(long roomId)
         {
             int index = 0;
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
                 if (roomsEnter.RoomId == roomId)
                 {

                     _stopAlarm = new Thread(SetAlertFalse);
                     roomsEnter.Alarm = true;
                     _stopAlarm.Start(index);
                 }

                 index++;
             }
         }

         private string _portName;
         public void Fill(int floorId)
         {
             int roomsCount = 0;
             int roomsIsProtected = 0;
             int roomsLightOn = 0;

             AlfaEntities alfaEntities = new AlfaEntities();

             Floors floor = (from floorse in alfaEntities.Floors
                            where floorse.FloorId == floorId 
                            select floorse).FirstOrDefault();

             _portName = floor.ComPort;

             IEnumerable<Rooms> rooms = from roomse in alfaEntities.Rooms
                                        where roomse.FloorId == floorId
                                        select roomse;

             //todo можно убрать вместе с ифами когда будут выставлены дефалтные значения в базе
             foreach (var room in rooms)
             {
                 roomsCount++;
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
                     if (room.IsProtected == true)
                     {
                         roomsIsProtected++;
                     }
                     if (room.LightOn == true)
                     {
                         roomsLightOn++;
                     }
                     
                     if (room.OnLine != null)
                         this.Add(new RoomsEnter(room.RoomId, (int) room.RoomNumber, (bool) room.IsProtected,
                                                 (bool)room.LightOn, (byte)room.ConrollerId, (bool)room.OnLine));

                 }
             }

             Counter counter = Counter.GetInstance();
             counter.RoomsCount = roomsCount;
             counter.RoomsOnline = this.Count;
             counter.RoomsLightOn = roomsLightOn;
             counter.RoomsProtected = roomsIsProtected;
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
             Counter.GetInstance().RoomsCount = _roomCollection.Count;
         }

         public static void AddByControllerNumber(string portName, byte controllerNumber)
         {
             AlfaEntities alfaEntities = new AlfaEntities();

             Rooms room = (from roomse in alfaEntities.Rooms.Include("Floors")
                           where roomse.Floors.ComPort == portName && roomse.ConrollerId == controllerNumber
                           select roomse).FirstOrDefault();

             if (room != null)
             {
                 if (room.IsProtected == null)
                 {
                     room.IsProtected = false;
                 }

                 if (room.LightOn == null)
                 {
                     room.LightOn = true;
                 }

                 if (room.RoomNumber != null && room.OnLine != null)
                 {
                     _roomCollection.Add(new RoomsEnter(room.RoomId, (int)room.RoomNumber, (bool)room.IsProtected, (bool)room.LightOn,
                                   (byte)room.ConrollerId, (bool)room.OnLine));
                 }
             }

             Counter.GetInstance().RoomsCount = _roomCollection.Count;
         }

         public static void UpdateKeys(string portName, byte controllerNumber)
         {
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
//                 if (roomsEnter.ControllerId == controllerNumber)
//                 {
                     roomsEnter.Keys.Fill(roomsEnter.RoomId);
//                     return;
//                 }
             }
         }

         public static Dictionary<long, bool> GetGuardState()
         {
             Dictionary<long, bool> state = new Dictionary<long, bool>();
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
                 state.Add(roomsEnter.RoomId, roomsEnter.GuardOn);
             }

             return state;
         }

         public static bool SetGuardState(Dictionary<long, bool> state)
         {
             foreach (RoomsEnter roomsEnter in _roomCollection)
             {
                 foreach (KeyValuePair<long, bool> roomsState in state)
                 {
                     if (roomsState.Key == roomsEnter.RoomId)
                     {
                         if (ServiceClient.GetInstance().ServerOnline)
                         {
                             ServiceClient.GetInstance().GetClientServiceClient().SetRoomToProtect(
                                 _roomCollection._portName, roomsEnter.ControllerId, roomsState.Value);
                             break;
                         }
                         else
                         {
                             return false;
                         }
                     }
                 }
             }

             return true;
         }

         public Dictionary<long, bool> GetLightState()
         {
             Dictionary<long, bool> state = new Dictionary<long, bool>();
             foreach (RoomsEnter roomsEnter in this)
             {
                 state.Add(roomsEnter.RoomId, roomsEnter.LightOn);
             }

             return state;
         }

         public bool SetLightState(Dictionary<long, bool> state)
         {
             foreach (RoomsEnter roomsEnter in this)
             {
                 foreach (KeyValuePair<long, bool> roomsState in state)
                 {
                     if (roomsState.Key == roomsEnter.RoomId)
                     {
                         if (ServiceClient.GetInstance().ServerOnline)
                         {
                             ServiceClient.GetInstance().GetClientServiceClient().SetLight(
                                 this._portName, roomsEnter.ControllerId, roomsState.Value);
                             break;
                         }
                         else
                         {
                             return false;
                         }
                     }
                 }
             }

             return true;
         }

         public static void ReloadData(int floorId)
         {
             _roomCollection.Fill(floorId);
         }
     }
}
