using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace ALFA_Client.Models
{
    class RHDB
    {
        public RHDB()
        {
        }

        public List<RHDBUser> GetUsers(int roomNumber)
        {
            string room = roomNumber.ToString();

            while (room.Length < 4)
            {
                room = "0" + room;
            }

            List<RHDBUser> users = new List<RHDBUser>();

            SqlConnection conn = new SqlConnection();

            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["RHDB"];

            if (settings != null)
            {
                conn.ConnectionString = settings.ConnectionString;

                SqlCommand myCommand = conn.CreateCommand();
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandText = "Alfa_Locks2";
                myCommand.Parameters.Add("Numroom", SqlDbType.VarChar).Value = room;
                try
                {
                    conn.Open();

                }
                catch (Exception)
                {
                    return null;
                    //throw new Exception("Не удалось подключиться к базе данных гостиницы");
                }

                SqlDataReader dataReader = myCommand.ExecuteReader();
                while (dataReader.Read())
                {
                    var user = new RHDBUser();
                    user.RoomNumber = int.Parse(dataReader["nuroom"].ToString());
                    user.FIO = dataReader["fio"].ToString();
                    user.Arriv = DateTime.Parse(dataReader["Arriv"].ToString());
                    user.Depar = DateTime.Parse(dataReader["Depar"].ToString());
                    user.Sex = byte.Parse(dataReader["sex"].ToString());
                    user.Guestidn = long.Parse(dataReader["Guestidn"].ToString());
                    users.Add(user);
                }
                dataReader.Close();
                conn.Close();


            }
            return users;
        }
    }

    public class RHDBUser
    {
        public int RoomNumber { set; get; }
        public string FIO { set; get; }
        public DateTime Arriv { set; get; }
        public DateTime Depar { set; get; }
        public byte Sex { set; get; }
        public long Guestidn { set; get; }
    }
}
