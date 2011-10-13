using System;
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
            _sortedCollectionViewSource.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
            _sortedCollectionViewSource.Source = this;

            return _sortedCollectionViewSource;
        }

        public void Info(string message)
        {
            Add(new AlfaClientLog(message));
        }
    }

    public class AlfaClientLog: INotifyPropertyChanged
    {
        public AlfaClientLog(string message)
        {
            Time = DateTime.Now.ToString("dd.MM  hh:mm");
            Message = message;
        }

        public string Time
        {
            get; set;
        }

        public string Message { get; set; }

        private bool _isRead;
        public bool IsRead
        {
            get { return _isRead; }
            set
            {
                _isRead = value;
                NotifyPropertyChanged("IsRead");
            }
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
}
