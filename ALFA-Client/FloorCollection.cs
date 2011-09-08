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

    public class FloorsEnter : INotifyPropertyChanged
    {
        public FloorsEnter(int floorId, string floor, string comport, bool online, bool ispolling)
        {
            _floorId = floorId;
            _floor = floor;
            _comport = comport;
            _online = online;
            _ispolling = ispolling;
        }

        private string _comport;
        private bool _online;
        private bool _ispolling;
        private int _floorId;
        private string _floor;



        public int FloorId
        {
            get { return _floorId; }
            set { _floorId = value; }
        }
        public string Floor
        {
            get { return _floor; }
            set { _floor = value; }
        }
        public string ComPort
        {
            get { return _comport; }
            set { _comport = value; }
        }
        public bool Online
        {
            get { return _online; }
            set { _online = value; }
        }
        public bool IsPolling
        {
            get { return _ispolling; }
            set { _ispolling = value; }
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

    public class FloorCollection : ObservableCollection<FloorsEnter>
    {
        public FloorCollection()
        {
            _floorCollection = this;
        }

        private static FloorCollection _floorCollection;

        public void Fill()
        {
            AlfaEntities alfaEntities = new AlfaEntities();
            IEnumerable<Floors> floors = from floorses in alfaEntities.Floors
                                        select floorses;



            foreach (var floor in floors)
            {
                    if (floor.IsPolling != null)
                        if (floor.Online != null)
                            this.Add(new FloorsEnter(floor.FloorId, floor.FloorName, floor.ComPort, (bool)floor.Online,
                                                     (bool)floor.IsPolling));
            }

            // в случае пустых данных в базе сохранить дефалтные данные
            //todo можно убрать вместе с ифами когда будут выставлены дефалтные значения в базе
        }
    }

}
