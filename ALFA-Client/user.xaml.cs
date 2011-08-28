using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Linq;


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

            _clientService = ServiceClient.GetInstance().GetClientServiceClient();

            RommsL = this.Resources["RoomsDataSource"] as RoomCollection;
            _keysCollectionL = this.Resources["KeysDataSource"] as KeysCollection;

            _floorId = floor;

            //определение сом порта этажа
            // todo надо переписать под универсальную строку коннекта когда будет создан админ
//            string conn = @"Data Source=microsoft-pc;Initial Catalog=ALFA;Integrated Security=True";
//            SqlConnection sqlComPort = new SqlConnection(conn);
//            sqlComPort.Open();
//            string infoAboutComPort = @"SELECT ComPort FROM Floors
//                                          WHERE (FloorName = '" + _floorId + "')";
//            SqlCommand sqlinfoAboutComPort = new SqlCommand(infoAboutComPort, sqlComPort);
//            SqlDataReader rezultat = sqlinfoAboutComPort.ExecuteReader();
//            rezultat.Read();
//            _yComPort = (string)rezultat["ComPort"];
//            rezultat.Close();
//            sqlComPort.Close();

            
            
            
            // ну согласись, лучше чем верхняя портянка, даже если предположить что ты не знаешь Linq то тут все понятно
            AlfaEntities alfaEntities = new AlfaEntities();

            // todo ошибочно считается что не может быть ситуации когда забит этаж и не указан ком порт, надо обработать это исключение
            _yComPort = (from floorse in alfaEntities.Floors
                         where floorse.FloorId == _floorId
                         select floorse.ComPort).FirstOrDefault();
        }

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

        RoomCollection RommsL;
        KeysCollection _keysCollectionL;
        
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            RommsL.Fill(_floorId);
// (wtf) не правильно. а если сервер не отвечает????
            //_clientService.Join(_yComPort);
            // todo надо повешать какой индикатор что сервак не отвечает и нужен метод ping
            //bool b = _clientService.Join(_yComPort);
            //ConnectToService();
            ButtonSetkey.IsEnabled = false;
            ButtonUnsetkey.IsEnabled = false;
            comboBoxTypeKey.IsEnabled = false;
           //comboBoxMinutes.IsEnabled = false;
           // comboBoxHour.IsEnabled = false;
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
                else
                {
                    Thread.Sleep(3000);
                    _clientService.Join(_yComPort);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }



        private void ListBox1MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
            RoomsEnter roomSelection = listBox1.SelectedItem as RoomsEnter;
            if (roomSelection != null) 
                _keysCollectionL.Fill(roomSelection.RoomId);
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
                    bool setkey=_clientService.SetKey(_key, (byte) keySelectionToset.Number, _yComPort, (byte) roomSelectionTosetKey.ControllerId,
                                                      textBoxFIO.Text, selectedDate);
                
                    if (setkey == true)
                    {
                        RoomsEnter roomSelection = listBox1.SelectedItem as RoomsEnter;
                        if (roomSelection != null) 
                            _keysCollectionL.Fill(roomSelection.RoomId);
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
                bool unsetkey = _clientService.UnsetKey(_yComPort, (byte) roomSelectionToUnsetKey.ControllerId, (byte) keySelection.Number);
                if (unsetkey == true)
                {
                    RoomsEnter roomSelection = listBox1.SelectedItem as RoomsEnter;
                    if (roomSelection != null) 
                        _keysCollectionL.Fill(roomSelection.RoomId);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить ключ.");
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            foreach (RoomsEnter roomsEnter in RommsL)
            {
                    roomsEnter.Alarm = true;
            }
            
        }


    }
}
