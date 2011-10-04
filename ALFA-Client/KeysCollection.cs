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
        public KeysEnter(int number, int typeKey, string name, string key, DateTime? finishDate)
        {
            _number = number;
            _typeKey = typeKey;
            _name = name;
            _key = key;
            _finishDate = finishDate;
        }

        private int _number;
        private int _typeKey;
        private string _name;
        private string _key;
        private DateTime? _finishDate;

        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public int TypeKey
        {
            get { return _typeKey; }
            set
            {
                _typeKey = value;
                DisplayTypeKey = value.ToString();
            }
        }

        public string DisplayTypeKey
        {
            get
            {
                return _keyTypes[_typeKey];
            }
            set
            {
                _typeKey = int.Parse(value);
            }
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

        private string[] _keyTypes = new string[] { "Гостевой", "Горничный", "Тех. служебный"};
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
