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
        public UsersEnter(string userName, string login, string pass, int floorId, string floor, int userid)
        {
            _userName = userName;
            _userid = userid;
            _login = login;
            _pass = pass;
            _floorId = floorId;
            _floor = floor;
        }

        private string _userName;
        private int _userid;
        private string _login;
        private string _pass;
        private int _floorId;
        private string _floor;


        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        public int UserId
        {
            get { return _userid; }
            set { _userid = value; }
        }
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public string Pass
        {
            get { return _pass; }
            set { _pass = value; }
        }
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

        public void Fill()
        {
            AlfaEntities alfaEntities = new AlfaEntities();
            IEnumerable<Users> users = from userses in alfaEntities.Users.Include("Floors")
                                      select userses;



            foreach (var user in users)
            {
                if (user.Remove == null && user.Type == 1)
                {
                    if (user.FloorId != null)
                        this.Add(new UsersEnter(user.UserName, user.Login, user.Password, (int)user.FloorId, user.Floors.FloorName, user.UserId));
                }
            }

            // в случае пустых данных в базе сохранить дефалтные данные
            //todo можно убрать вместе с ифами когда будут выставлены дефалтные значения в базе
        }
    }

}
