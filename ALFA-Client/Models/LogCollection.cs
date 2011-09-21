using System;
using System.Collections.ObjectModel;

namespace ALFA_Client.Models
{
    public class LogCollection : ObservableCollection<AlfaClientLog>
    {
        static LogCollection _instance = null;
        static readonly object Lock = new object();

        public LogCollection()
        {
        }

        public static LogCollection GetInstance()
        {
            lock (Lock)
            {
                if (_instance == null)
                {
                    _instance = new LogCollection();
                }
                return _instance;
            }
        }
    }

    public class AlfaClientLog
    {
        public AlfaClientLog(DateTime time, string eve)
        {
            Time = time;
            Event = eve;
//            Room = room;
        }
        public DateTime Time { get; set; }
        public string Event { get; set; }
        public string Room { get; set; }
    }
}
