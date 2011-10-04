using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace ALFA_Client.Models
{
    public class LogCollection : MTObservableCollection<AlfaClientLog>
    {
        static LogCollection _instance = null;
        static readonly object Lock = new object();

        public LogCollection()
        {
        }


        private CollectionViewSource _sortedCollectionViewSource;
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
        public CollectionViewSource GetCollectionView()
        {
            _sortedCollectionViewSource = new CollectionViewSource();
            _sortedCollectionViewSource.SortDescriptions.Add(new SortDescription("Time",ListSortDirection.Descending));
            _sortedCollectionViewSource.Source = this;

            return _sortedCollectionViewSource;
        }

        public void Info(string message)
        {
            //todo вставить норм звук 
            System.Media.SystemSounds.Beep.Play();
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
        public bool IsRead { get; set; }
    }
}
