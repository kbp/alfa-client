using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ALFA_Client.ClientServiceReference;

namespace ALFA_Client
{
    /// <summary>
    /// Interaction logic for AddKeyWindow.xaml
    /// </summary>
    public partial class AddKeyWindow : Window
    {
        public AddKeyWindow(int floorId, string portName, ClientServiceClient clientService)
        {
            InitializeComponent();
            _floorId = floorId;
            _clientService = clientService;
            _portName = portName;
        }

        private RoomCollection _roomCollection;
        private int _floorId;
        private string _portName;

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            _roomCollection = Resources["RoomCollectionDataSource"] as RoomCollection;
            
            if (_roomCollection != null)
            {
                _roomCollection.Fill(_floorId);
            }

            DateTime today = DateTime.Now.AddDays(1);
            dameer1.Value = new DateTime(today.Year, today.Month, today.Day, 12, 0, 0);

            radioButton1.IsChecked = true;
        }

        private ClientServiceClient _clientService;

        private void RadioButton9Checked(object sender, RoutedEventArgs e)
        {
            foreach (RoomsEnter roomsEnter in _roomCollection)
            {
                roomsEnter.IsSelected = true;
            }
        }

        private void ButtonReadKeyClick(object sender, RoutedEventArgs e)
        {
            
            try
            {
                _key = _clientService.ReadKey(_portName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

            textBoxKey.Text = BitConverter.ToString(_key);
        }
        private byte[] _key = new byte[5];

        private void ButtonSetKeyClick(object sender, RoutedEventArgs e)
        {
            label2.Visibility = System.Windows.Visibility.Visible;
            if (dameer1.Value == null && dameer1.Value <= DateTime.Now)
            {
                MessageBox.Show("Не верно указан срок действия ключа");
            }

            byte number = 3;
            // todo внимание!!! говнокод

            if (radioButton1.IsChecked == true) number = 3;
            if (radioButton2.IsChecked == true) number = 4;
            if (radioButton3.IsChecked == true) number = 5;
            if (radioButton4.IsChecked == true) number = 6;
            if (radioButton5.IsChecked == true) number = 7;
            if (radioButton6.IsChecked == true) number = 8;
            if (radioButton7.IsChecked == true) number = 9;
            if (radioButton8.IsChecked == true) number = 10;
            if (radioButton9.IsChecked == true) number = 11;

            List<byte> checkedControllers = new List<byte>();

            foreach (RoomsEnter roomsEnter in _roomCollection)
            {
                if (roomsEnter.IsSelected)
                {
                    checkedControllers.Add(roomsEnter.ControllerId);
                }
            }

            try
            {
                if (_clientService.SetKeyMass(_key, _portName, checkedControllers.ToArray(), number, "Горничный", 1,
                          (DateTime)dameer1.Value))
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось назначить ключ");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Сервер не успел ответить");
            }
        }

        private void ButtonUnsetKeyClick(object sender, RoutedEventArgs e)
        {
            label2.Visibility = Visibility.Visible;

            byte number = 3;
            // todo внимание!!! говнокод

            if (radioButton1.IsChecked == true) number = 3;
            if (radioButton2.IsChecked == true) number = 4;
            if (radioButton3.IsChecked == true) number = 5;
            if (radioButton4.IsChecked == true) number = 6;
            if (radioButton5.IsChecked == true) number = 7;
            if (radioButton6.IsChecked == true) number = 8;
            if (radioButton7.IsChecked == true) number = 9;
            if (radioButton8.IsChecked == true) number = 10;
            if (radioButton9.IsChecked == true) number = 11;

            List<byte> checkedControllers = new List<byte>();

            foreach (RoomsEnter roomsEnter in _roomCollection)
            {
                if (roomsEnter.IsSelected)
                {
                    checkedControllers.Add(roomsEnter.ControllerId);
                }
            }

            try
            {
                if (_clientService.UnsetKeyMass(_portName, checkedControllers.ToArray(), number))
                {
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось отменить ключ");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Сервер не успел ответить");
            }
        }


        private void CheckBoxCheckAllClick(object sender, RoutedEventArgs e)
        {
            if (_roomCollection == null) return;
            foreach (RoomsEnter roomsEnter in _roomCollection)
            {
                if (checkBoxCheckAll.IsChecked != null)
                    roomsEnter.IsSelected = (bool)checkBoxCheckAll.IsChecked;
            }
        }
    }
}
