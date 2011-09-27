using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ALFA_Client.Models
{
    public class Counter : INotifyPropertyChanged
    {
        static Counter _instance = null;
        static readonly object Lock = new object();

        public Counter()
        {
        }

        public static Counter GetInstance()
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new Counter();
                }
                return _instance;
            }
        }

        private int _roomsCount;
        public int RoomsCount
        {
            get { return _roomsCount; }
            set
            {
                _roomsCount = value;
                NotifyPropertyChanged("RoomsCount");
            }
        }

        private int _roomsOnline;
        public int RoomsOnline
        {
            get { return _roomsOnline; }
            set
            {
                _roomsOnline = value;
                NotifyPropertyChanged("RoomsOnline");
            }
        }

        private int _roomsProtected;
        public int RoomsProtected
        {
            get { return _roomsProtected; }
            set
            {
                _roomsProtected = value;
                NotifyPropertyChanged("RoomsProtected");
            }
        }

        private int _roomsLightOn;
        public int RoomsLightOn
        {
            get { return _roomsLightOn; }
            set
            {
                _roomsLightOn = value;
                NotifyPropertyChanged("RoomsLightOn");
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
