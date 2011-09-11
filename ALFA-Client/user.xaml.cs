using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using NLog;


namespace ALFA_Client
{
    using Entities;
    using ClientServiceReference;
    /// <summary>
    /// Interaction logic for user.xaml
    /// </summary>
    public partial class User
    {
        private readonly ClientServiceClient _clientService;
        public User(string username, bool guartOn, bool alarmOn, bool keyOnOff, bool datX, int cellgroup, string catRoom, int floor)
        {
            InitializeComponent();

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("user windows init");
            _clientService = ServiceClient.GetInstance().GetClientServiceClient();
//            ррбля!
            _roomCollection = this.Resources["RoomsDataSource"] as RoomCollection;
//            _keysCollectionL = this.Resources["KeysDataSource"] as KeysCollection;

            _floorId = floor;

            // ну согласись, лучше чем верхняя портянка, даже если предположить что ты не знаешь Linq то тут все понятно
            AlfaEntities alfaEntities = new AlfaEntities();

            // todo ошибочно считается что не может быть ситуации когда забит этаж и не указан ком порт, надо обработать это исключение
            _yComPort = (from floorse in alfaEntities.Floors
                         where floorse.FloorId == _floorId
                         select floorse.ComPort).FirstOrDefault();

            _connection = new Thread(ConnectToService);
            _connection.Start();
        }

        private Thread _connection;

        private int _floorId;

        public int Floor
        {
            get { return _floorId; }
            set { _floorId = value; }
        }


        private string _yComPort;
        public string ComPort
        {
            get { return _yComPort; }
            set { _yComPort = value; }
        } 

        private RoomCollection _roomCollection;
        
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _roomCollection.Fill(_floorId);
            ButtonSetkey.IsEnabled = false;
            ButtonUnsetkey.IsEnabled = false;
            comboBoxTypeKey.IsEnabled = false;
            textBoxFIO.IsEnabled = false;
            dameer1.IsEnabled = false;
            textBoxSetKey.IsEnabled = false;
        }

        private void ConnectToService()
        {
            try
            {
                if (_clientService.Join(_yComPort))
                {
                    return;
                }
                
                Thread.Sleep(3000);
                _clientService.Join(_yComPort);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void ListBox1MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
//            RoomsEnter roomSelection = listBox1.SelectedItem as RoomsEnter;
//            if (roomSelection != null) 
//                _keysCollectionL.Fill(roomSelection.RoomId);
        }

        private void ListBox2MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
            KeysEnter keySelection = listBox2.SelectedItem as KeysEnter;
            if (keySelection != null)
            {
                if (keySelection.Key == "-")
                {
                    ButtonSetkey.IsEnabled = false;
                    ButtonUnsetkey.IsEnabled = false;
                    comboBoxTypeKey.IsEnabled = false;
                   // comboBoxMinutes.IsEnabled = false;
                   // comboBoxHour.IsEnabled = false;
                    textBoxFIO.IsEnabled = false;
                    textBoxSetKey.Text = "Пусто";
                    dameer1.IsEnabled = false;
                    comboBoxTypeKey.SelectedIndex = 1;
                 //   comboBoxHour.SelectedIndex = 13;
                  //  comboBoxMinutes.SelectedIndex = 2;
                }
                else
                {
                    ButtonSetkey.IsEnabled = true;
                    ButtonUnsetkey.IsEnabled = true;
                    comboBoxTypeKey.IsEnabled = true;
                 //   comboBoxMinutes.IsEnabled = true;
                 //   comboBoxHour.IsEnabled = true;
                    textBoxFIO.IsEnabled = true;
                    dameer1.IsEnabled = true;
                    textBoxSetKey.Text = keySelection.Key;
                    textBoxFIO.Text = keySelection.Name;
                    comboBoxTypeKey.SelectedIndex = keySelection.TipeKey;
                  //  dameer1.SelectedDate = keySelection.FinishDate;
                    //todo дописать часы и минуты
                }
            }
        }

        private byte[] _key;
        private byte[] _yControllerId;
        
        private void ButtonReadkeyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _key = _clientService.ReadKey(_yComPort);
                textBoxSetKey.Text = BitConverter.ToString(_key);
                ButtonSetkey.IsEnabled = true;
                comboBoxTypeKey.IsEnabled = true;
             //   comboBoxMinutes.IsEnabled = true;
            //    comboBoxHour.IsEnabled = true;
                textBoxFIO.IsEnabled = true;
                dameer1.IsEnabled = true;
            }
            catch (Exception)
            {

                MessageBox.Show("Не удалось считать ключ!");
            }
        }

        private void ButtonSetkeyClick(object sender, RoutedEventArgs e)
        {
            KeysEnter keySelectionToset = listBox2.SelectedItem as KeysEnter;
            RoomsEnter roomSelectionTosetKey = listBox1.SelectedItem as RoomsEnter;
            //todo дописать условие проверки установленной даты и времени (д.б. больше текущего значения)
            if (textBoxFIO.Text != "")
            {
                DateTime selectedDate = new DateTime();
               // if (calendar1.SelectedDate != null)
                //{
                    //selectedDate = (DateTime)calendar1.SelectedDate;
                    //selectedDate.AddHours(double.Parse(comboBoxHour.SelectedValue.ToString()));
                    //selectedDate.AddMinutes(double.Parse(comboBoxMinutes.SelectedValue.ToString()));
                
                    if (dameer1.Value != null) selectedDate=(DateTime)dameer1.Value;
                //}


               // string dataFinish = calendar1.SelectedDate.ToString();
                //todo дописать часы и минуты
                //почему номер ячейки в контроллере байтовский???????????????
                if (keySelectionToset != null && roomSelectionTosetKey != null)
                {
                    bool setkey=_clientService.SetKey(_key, (byte) keySelectionToset.Number, _yComPort, roomSelectionTosetKey.ControllerId,
                                                      textBoxFIO.Text, (byte)comboBoxTypeKey.SelectedIndex, selectedDate);
                
                    if (setkey == true)
                    {
                        RoomsEnter roomSelection = listBox1.SelectedItem as RoomsEnter;
                        if (roomSelection != null) 
                            roomSelection.Keys.Fill(roomSelection.RoomId);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось записать ключ.");
                    }
                }
            }
            else MessageBox.Show("Не введено имя гостя, либо указаны не верно Дата и время отмены ключа.");
        }

        private void ButtonUnsetkeyClick(object sender, RoutedEventArgs e)
        {
            KeysEnter keySelection = listBox2.SelectedItem as KeysEnter;
            RoomsEnter roomSelectionToUnsetKey = listBox1.SelectedItem as RoomsEnter;

            //почему тут уже ControllerId byte а в setkey был int???????????
            if (roomSelectionToUnsetKey != null && keySelection != null)
            {
                bool unsetkey = _clientService.UnsetKey(_yComPort, roomSelectionToUnsetKey.ControllerId, (byte) keySelection.Number);
                if (unsetkey)
                {
                    RoomsEnter roomSelection = listBox1.SelectedItem as RoomsEnter;
                    if (roomSelection != null) 
                        roomSelection.Keys.Fill(roomSelection.RoomId);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить ключ.");
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            foreach (RoomsEnter roomsEnter in _roomCollection)
            {
                roomsEnter.Alarm = true;
            }
        }
    }
}
