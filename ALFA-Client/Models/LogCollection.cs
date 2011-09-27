using System;
using System.Collections.ObjectModel;

namespace ALFA_Client.Models
{
    public class LogCollection : MTObservableCollection<AlfaClientLog>
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

        public void Info(string message)
        {
            Add(new AlfaClientLog(message));
        }
    }

    public class AlfaClientLog
    {
        public AlfaClientLog(string message)
        {
            Time = DateTime.Now;
            Message = message;
        }
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
