using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using ALFA_Client.Entities;
using ALFA_Client.Models;

namespace ALFA_Client
{
    public class KeysEnter
    {
        public KeysEnter(int number, int tipeKey, string name, string key, DateTime? finishDate)
        {
            _number = number;
            _tipeKey = tipeKey;
            _name = name;
            _key = key;
            _finishDate = finishDate;
        }

        private int _number;
        private int _tipeKey;
        private string _name;
        private string _key;
        private DateTime? _finishDate;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public int TipeKey
        {
            get { return _tipeKey; }
            set { _tipeKey = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public DateTime? FinishDate
        {
            get { return _finishDate; }
            set { _finishDate = value; }
        }
    }

    public class KeysCollection : MTObservableCollection<KeysEnter>
    {
        public void Fill(int roomId)
        {
            base.Clear();

            AlfaEntities alfaEntities = new AlfaEntities();

            IEnumerable<Keys> keyses = from key in alfaEntities.Keys
                                       where key.RoomId == roomId
                                       select key;

            
            foreach (Keys keyse in keyses)
            {
                base.Add(new KeysEnter(keyse.CellNumber, keyse.Type, keyse.FIO, keyse.keyCode, keyse.EndDate));
            }
        }
    }
}
