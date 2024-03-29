﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.Sql;
using System.Data.SqlClient;

namespace ALFA_Client
{
    using Entities;

    public class RoomsEnter
    {
        public RoomsEnter(int roomId, int room, bool guardOn, bool lightOn, int controllerId)
        {
            _room = room;
            _roomId = roomId;
            _guardOn = guardOn;
            _lightOn = lightOn;
            _controllerId = controllerId;
        }

        private int _room;
        private int _roomId;
        private bool _guardOn;
        private bool _lightOn;
        private int _controllerId;

        public int Room
        {
            get { return _room; }
            set { _room = value; }
        }

        public int RoomId
        {
            get { return _roomId; }
            set { _roomId = value; }
        }        
        
        public bool GuardOn
        {
            get { return _guardOn; }
            set { _guardOn = value; }
        }

        public bool LightOn
        {
            get { return _lightOn; }
            set { _lightOn = value; }
        }

        public int ControllerId
        {
            get { return _controllerId; }
            set { _controllerId = value; }
        }
    }

     public class RoomCollection : ObservableCollection<RoomsEnter>
     {
        //const string Conn = @"Data Source=microsoft-pc;Initial Catalog=ALFA;Integrated Security=True";
//        
//         public void Fill(int floorid)
//        {
//            SqlConnection sqlRoom = new SqlConnection(Conn);
//            sqlRoom.Open();
//            string infoAboutRooms = @"SELECT RoomId, RoomNumber, IsProtected, LightOn, ControllerId FROM RoomCollection
//                                 WHERE (FloorId = '" + floorid + "')";
//            SqlCommand sqlInfoAboutRooms = new SqlCommand(infoAboutRooms, sqlRoom);
//            SqlDataReader tabRooms = sqlInfoAboutRooms.ExecuteReader();
//
//            while (tabRooms.Read())
//            {
//                base.Add(new RoomsEnter((int)tabRooms["RoomId"], (int)tabRooms["RoomNumber"], (bool)tabRooms["IsProtected"], (bool)tabRooms["LightOn"], (int)tabRooms["ControllerId"]));
//            }
//
//            tabRooms.Close();
//            sqlRoom.Close();
//        }

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
                     this.Add(new RoomsEnter(room.RoomId, (int) room.RoomNumber, (bool) room.IsProtected,
                                             (bool) room.LightOn, room.ConrollerId));
                 }
             }

             // в случае пустых данных в базе сохранить дефалтные данные
             //todo можно убрать вместе с ифами когда будут выставлены дефалтные значения в базе
             alfaEntities.SaveChanges();
         }
     }

}
