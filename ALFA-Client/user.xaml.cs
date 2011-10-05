using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using ALFA_Client.Models;
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
        private ClientServiceClient _clientService;
        public User(string username, bool guartOn, bool alarmOn, bool keyOnOff, bool datX, int cellgroup, string catRoom, int floor)
        {

            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("user windows init");

            InitializeComponent();


            _floorId = floor;

            // ну согласись, лучше чем верхняя портянка, даже если предположить что ты не знаешь Linq то тут все понятно
            AlfaEntities alfaEntities = new AlfaEntities();

            // todo ошибочно считается что не может быть ситуации когда забит этаж и не указан ком порт, надо обработать это исключение
            _portName = (from floorse in alfaEntities.Floors
                         where floorse.FloorId == _floorId
                         select floorse.ComPort).FirstOrDefault();
        }

        private LogCollection _alfaEventLog;

        private int _floorId;

        public int Floor
        {
            get { return _floorId; }
            set { _floorId = value; }
        }


        private static string _portName;
        public static string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        } 

        private RoomCollection _roomCollection;
        
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _clientService = ServiceClient.GetInstance().GetClientServiceClient();
            _roomCollection = Resources["RoomsDataSource"] as RoomCollection;

            _alfaEventLog = LogCollection.GetInstance();
            listBoxLog.DataContext = _alfaEventLog.GetCollectionView();
            gridCounter.DataContext = Counter.GetInstance();

            checkBoxConnection.DataContext = ServiceClient.GetInstance();

            if (_roomCollection != null)
            {
                _roomCollection.Fill(_floorId);
            }

            ButtonSetkey.IsEnabled = false;
            ButtonUnsetkey.IsEnabled = false;
            comboBoxTypeKey.IsEnabled = false;
            textBoxFIO.IsEnabled = false;
            dameer1.IsEnabled = false;
            textBoxSetKey.IsEnabled = false;

            _alfaEventLog.Info("Программа запущена");
            checkBoxConnection.IsChecked = true;
            _timerOnlineStatus = new Timer(CheckOnlineStatus, null, 0, 30000);
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
                    comboBoxTypeKey.SelectedIndex = keySelection.TypeKey;
                  //  dameer1.SelectedDate = keySelection.FinishDate;
                    //todo дописать часы и минуты
                }
            }
        }

        private byte[] _key;

        private void ButtonReadkeyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                
                try
                {
                    _key = _clientService.ReadKey(_portName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
                
                textBoxSetKey.Text = BitConverter.ToString(_key);
                ButtonSetkey.IsEnabled = true;
                comboBoxTypeKey.IsEnabled = true;
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
                    bool setkey=_clientService.SetKey(_key, (byte) keySelectionToset.Number, _portName, roomSelectionTosetKey.ControllerId,
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

            if (roomSelectionToUnsetKey != null && keySelection != null)
            {
                bool unsetkey = _clientService.UnsetKey(_portName, roomSelectionToUnsetKey.ControllerId, (byte) keySelection.Number);
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

        private void ButtonCheckKeyClick(object sender, RoutedEventArgs e)
        {
            if (textBoxSetKey.Text != "00")
            {
                AlfaEntities alfaEntities = new AlfaEntities();
                IQueryable<Keys> keys = from keyes in alfaEntities.Keys.Include("Rooms")
                                        where (keyes.keyCode == textBoxSetKey.Text)
                                        select keyes;

                if (keys.Count() > 5)
                {
                    MessageBox.Show(
                        "Данный ключ закреплен за множеством комнат, для уточнения информации обратитесь к администратору");
                    return;
                }

                bool first = true;
                string message = "";
                foreach (Keys keyse in keys)
                {
                    if (first)
                    {
                        message = "Код ключа: " + keyse.keyCode + "\n";
                        first = false;
                    }
                    message += "ФИО: " + keyse.FIO + "   " + "Комната №" + keyse.Rooms.RoomNumber + "\n";
                }

                MessageBox.Show(message);
            }
        }

        private static bool _serverOnline;
        public static bool ServerOnline
        {
            get { return _serverOnline; }
            set
            {
                ServiceClient.GetInstance().ServerOnline = value;
                System.Diagnostics.Debug.WriteLine(value);
                _serverOnline = value;
            }
        }

        private static bool _isJoin = false;
        private Timer _timerOnlineStatus;
        private static void CheckOnlineStatus(object state)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool online = false;
            try
            {
//                if (_isJoin)
//                {
//                    online = ServiceClient.GetInstance().GetClientServiceClient().Ping();
//                }
//                else
//                {
                    online = ServiceClient.GetInstance().GetClientServiceClient().Join(_portName);
//                }
            }
            catch (Exception)
            {
//                _isJoin = false;
//                try
//                {
//                    online = ServiceClient.GetInstance().GetClientServiceClient().Join(_portName);
//                }
//                catch (Exception)
//                {
                    ServerOnline = false;
//                }
            }

            if (online)
            {
                ServerOnline = true;
            }
            else
            {
                ServerOnline = false;
            }
        }

        private void ListBoxLogSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlfaClientLog selectedItem = listBoxLog.SelectedItem as AlfaClientLog;
            if (selectedItem != null)
            {
                selectedItem.IsRead = false;
            }
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.F4 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // включить свет на этаже
                MessageBox.Show("shift + f4");
                if (ServiceClient.GetInstance().ServerOnline)
                {
                    ServiceClient.GetInstance().GetClientServiceClient().SetAllRoomLight(_portName, true);
                }
            }
            if (e.Key == Key.F5 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // выключить свет на этаже
                if (ServiceClient.GetInstance().ServerOnline)
                {
                    ServiceClient.GetInstance().GetClientServiceClient().SetAllRoomLight(_portName, false);
                }
            }
            if (e.Key == Key.F6 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // сохранить состояние света
                TextWriter textWriter = new StreamWriter("protected.txt", false, Encoding.UTF8);

                Dictionary<long, bool> state = _roomCollection.GetLightState();

                foreach (KeyValuePair<long, bool> keyValuePair in state)
                {
                    textWriter.WriteLine("" + keyValuePair.Key + "," + keyValuePair.Value);
                }
                textWriter.Flush();
                textWriter.Close();

            }
            if (e.Key == Key.F7 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // загрузить сохраненное состояние света
            }


            if (e.Key == Key.F4)
            {
                if (ServiceClient.GetInstance().ServerOnline)
                {
                    ServiceClient.GetInstance().GetClientServiceClient().SetAllRoomToProtect(_portName, true);
                }
            }
            if (e.Key == Key.F5)
            {
                if (ServiceClient.GetInstance().ServerOnline)
                {
                    ServiceClient.GetInstance().GetClientServiceClient().SetAllRoomToProtect(_portName, false);
                }
            }
            if (e.Key == Key.F6)
            {
            }
            if (e.Key == Key.F7)
            {
            }
        }
    }
}
