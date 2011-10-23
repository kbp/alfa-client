using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void getKeyParams(int roomNumber)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["RHDB"];
            SqlCommand myCommand = conn.CreateCommand();
            myCommand.CommandType = CommandType.StoredProcedure;
            myCommand.CommandText = "Alfa_Locks2";
            myCommand.Parameters.Add("Numroom", SqlDbType.VarChar).Value = roomNumber.ToString();
            conn.Open();

            SqlDataReader dataReader = myCommand.ExecuteReader();
            while (dataReader.Read())
            {
//                listBox1.Items.Add(dataReader.GetString(1) + "" + dataReader.GetString(2));
            }
            dataReader.Close();
            conn.Close();
        }
    }
}
