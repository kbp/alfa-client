using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ALFA_Client.ClientServiceReference;

namespace ALFA_Client
{
    using Entities;

    public class UsersEnter : INotifyPropertyChanged
    {
        public UsersEnter(int roomId, int room, bool guardOn, bool lightOn, byte controllerId)
        {
            _room = room;
            _roomId = roomId;
            _guardOn = guardOn;
            _lightOn = lightOn;
            _controllerId = controllerId;
            _alarm = false;
        }

        private int _room;
        private int _roomId;
        private bool _guardOn;
        private bool _lightOn;
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
                _guardOn = value;
                AlfaEntities alfaEntities = new AlfaEntities();

                var room = (from rooms in alfaEntities.Rooms.Include("Floor")
                            where rooms.RoomId == _roomId
                            select rooms).FirstOrDefault();
                if (room != null)
                {
                    ServiceClient.GetInstance().GetClientServiceClient().SetRoomToProtect(room.Floors.ComPort,
                                                                                          _controllerId, value);
                }
            }
        }

        public bool LightOn
        {
            get { return _lightOn; }
            set
            {
                _lightOn = value;
                AlfaEntities alfaEntities = new AlfaEntities();

                var room = (from rooms in alfaEntities.Rooms.Include("Floor")
                            where rooms.RoomId == _roomId
                            select rooms).FirstOrDefault();
                if (room != null)
                {
                    ServiceClient.GetInstance().GetClientServiceClient().SetLight(room.Floors.ComPort, _controllerId,
                                                                                  value);
                }
            }
        }

        public byte ControllerId
        {
            get { return _controllerId; }
            set { _controllerId = value; }
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

    public class UserCollection : ObservableCollection<UsersEnter>
    {
        public UserCollection()
        {
            _userCollection = this;
        }

        private static UserCollection _userCollection;


        public static void UpdateGerkon(long roomId)
        {
            foreach (UsersEnter UsersEnter in _userCollection)
            {
                if (UsersEnter.RoomId == roomId)
                {
                    UsersEnter.Alarm = true;
                }
            }
        }

        public void Fill(int floorId)
        {
            AlfaEntities alfaEntities = new AlfaEntities();
            IEnumerable<Rooms> rooms = from roomse in alfaEntities.Rooms
                                       where roomse.FloorId == floorId
                                       select roomse;

            foreach (var room in rooms)
            {
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
                    this.Add(new UsersEnter(room.RoomId, (int)room.RoomNumber, (bool)room.IsProtected,
                                            (bool)room.LightOn, (byte)room.ConrollerId));
                }
            }

            // в случае пустых данных в базе сохранить дефалтные данные
            //todo можно убрать вместе с ифами когда будут выставлены дефалтные значения в базе
            alfaEntities.SaveChanges();
        }
    }

}
