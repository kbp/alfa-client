using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    public partial class UserWindow
    {
        private ClientServiceClient _clientService;
        public UserWindow(string username, bool guartOn, bool alarmOn, bool keyOnOff, bool datX, int cellgroup, string catRoom, int floor)
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

            _alfaEventLog.Info("Программа запущена");
            _timerOnlineStatus = new Timer(CheckOnlineStatus, null, 0, 30000);
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

        private bool CheckPosibleSetKey()
        {
            bool error = false;
            string errorMessage = "";

            if (textBoxSetKey.Text == "" || textBoxSetKey.Text == "00" || textBoxSetKey.Text == "00-00-00-00-00")
            {
                error = true;
                errorMessage += "Не считан ключ.\n";
            }
            if (textBoxFIO.Text == "")
            {
                error = true;
                errorMessage += "Не указаны ФИО владельца ключа.\n";
            }
            if (dameer1.Value <= DateTime.Now)
            {
                error = true;
                errorMessage += "Дата окончания действия ключа меньше текущей даты.\n";
            }

            if (error)
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            return true;
        }
        private void ButtonSetkeyClick(object sender, RoutedEventArgs e)
        {
            if (!CheckPosibleSetKey())
            {
                return;
            }

            KeysEnter keySelectionToset = listBoxKeys.SelectedItem as KeysEnter;
            RoomsEnter roomSelectionTosetKey = listBoxRooms.SelectedItem as RoomsEnter;
            //todo дописать условие проверки установленной даты и времени (д.б. больше текущего значения)
            if (textBoxFIO.Text != "")
            {
                DateTime selectedDate = new DateTime();

                if (dameer1.Value != null) selectedDate=(DateTime)dameer1.Value;

                if (keySelectionToset != null && roomSelectionTosetKey != null)
                {
                    bool setkey=_clientService.SetKey(_key, (byte) keySelectionToset.Number, _portName, roomSelectionTosetKey.ControllerId,
                                                      textBoxFIO.Text, (byte)comboBoxTypeKey.SelectedIndex, selectedDate);
                
                    if (setkey == true)
                    {
                        RoomsEnter roomSelection = listBoxRooms.SelectedItem as RoomsEnter;
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


        private bool CheckPosibleUnsetKey()
        {
            bool error = false;
            string errorMessage = "";

            if (listBoxKeys.SelectedIndex < 0)
            {
                error = true;
                errorMessage += "Не выбран ключ.\n";
            }


            if (error)
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            return true;
        }

        private void ButtonUnsetkeyClick(object sender, RoutedEventArgs e)
        {
            if (!CheckPosibleUnsetKey())
            {
                return;
            }
            KeysEnter keySelection = listBoxKeys.SelectedItem as KeysEnter;
            RoomsEnter roomSelectionToUnsetKey = listBoxRooms.SelectedItem as RoomsEnter;

            if (roomSelectionToUnsetKey != null && keySelection != null)
            {
                bool unsetkey = _clientService.UnsetKey(_portName, roomSelectionToUnsetKey.ControllerId, (byte) keySelection.Number);
                if (unsetkey)
                {
                    RoomsEnter roomSelection = listBoxRooms.SelectedItem as RoomsEnter;
                    if (roomSelection != null) 
                        roomSelection.Keys.Fill(roomSelection.RoomId);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить ключ.");
                }
            }
        }

        private bool CheckPosibleCheckKey()
        {
            bool error = false;
            string errorMessage = "";

            if (textBoxSetKey.Text == "" || textBoxSetKey.Text == "00" || textBoxSetKey.Text == "00-00-00-00-00")
            {
                error = true;
                errorMessage += "Не считан ключ.\n";
            }

            if (error)
            {
                MessageBox.Show(errorMessage);
                return false;
            }

            return true;
        }

        private void ButtonCheckKeyClick(object sender, RoutedEventArgs e)
        {
            if (!CheckPosibleCheckKey())
            {
                return;
            }

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

        private static bool _serverOnline;
        public static bool ServerOnline
        {
            get { return _serverOnline; }
            set
            {
                ServiceClient.GetInstance().ServerOnline = value;
                Debug.WriteLine(value);
                _serverOnline = value;
            }
        }

//        private static bool _isJoin = false;
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
                selectedItem.IsRead = true;
            }
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.F4 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // включить свет на этаже
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
                TextWriter textWriter = new StreamWriter("light.txt", false, Encoding.UTF8);

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
                string[] stateFromFile = File.ReadAllLines("light.txt");
        
                Dictionary<long, bool> state = new Dictionary<long, bool>();
                
                string[] values;
                foreach (string s in stateFromFile)
                {
                    if (s != "")
                    {
                        values = s.Split(',');

                        state.Add(long.Parse(values[0]), bool.Parse(values[1]));
                    }
                }

                _roomCollection.SetLightState(state);
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
                // сохранить состояние охраны
                TextWriter textWriter = new StreamWriter("protected.txt", false, Encoding.UTF8);

                Dictionary<long, bool> state = _roomCollection.GetGuardState();

                foreach (KeyValuePair<long, bool> keyValuePair in state)
                {
                    textWriter.WriteLine("" + keyValuePair.Key + "," + keyValuePair.Value);
                }
                textWriter.Flush();
                textWriter.Close();
            }
            if (e.Key == Key.F7)
            {
                // загрузить сохраненное состояние охраны
                string[] stateFromFile = File.ReadAllLines("protected.txt");

                Dictionary<long, bool> state = new Dictionary<long, bool>();

                string[] values;
                foreach (string s in stateFromFile)
                {
                    if (s != "")
                    {
                        values = s.Split(',');

                        state.Add(long.Parse(values[0]), bool.Parse(values[1]));
                    }
                }

                _roomCollection.SetGuardState(state);
            }
        }

        private void ListBoxRoomsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RHDB rhdb = new RHDB();
            RHDBUser user;

            RoomsEnter roomsEnter = listBoxRooms.SelectedItem as RoomsEnter;

            if (roomsEnter != null)
            {
                user = rhdb.GetUser(roomsEnter.Room);
                if (user != null)
                {
                    textBoxFIO.Text = user.FIO;
                    // время отмены ключа выставляем в 12, не понятно по какой причине оно не возвращается таким из процедуры
                    dameer1.Value = new DateTime(user.Depar.Year, user.Depar.Month, user.Depar.Day, 12, 0, 0);
                }
                else
                {
                    textBoxFIO.Text = "";
                    DateTime today = DateTime.Now.AddDays(1);
                    dameer1.Value = new DateTime(today.Year, today.Month, today.Day, 12, 0, 0);
                    
                }
            }
            comboBoxTypeKey.SelectedIndex = 0;
        }

        private void ListBoxKeysSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeysEnter keysEnter = listBoxKeys.SelectedItem as KeysEnter;

            if (keysEnter != null && keysEnter.Name != "")
            {
                textBoxFIO.Text = keysEnter.Name;
                if (keysEnter.FinishDate == null)
                {
                    DateTime today = DateTime.Now.AddDays(1);
                    dameer1.Value = new DateTime(today.Year, today.Month, today.Day, 12, 0, 0);
                }
                else
                {
                    dameer1.Value = keysEnter.FinishDate;
                }
                
                textBoxSetKey.Text = keysEnter.Key;
                comboBoxTypeKey.SelectedIndex = keysEnter.TypeKey;
            }
        }

        private void ButtonAddServiceKeysClick(object sender, RoutedEventArgs e)
        {
            AddKeyWindow addKeyWindow = new AddKeyWindow(_floorId, _portName, _clientService);
            addKeyWindow.ShowDialog();
            _roomCollection.Fill(_floorId);
        }
    }
}
