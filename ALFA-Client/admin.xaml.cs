using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ALFA_Client.Entities;

namespace ALFA_Client
{
    public partial class Admin
    {
        public Admin(string username)
        {
            InitializeComponent();
            _usersL = new UserCollection();
            _usersL = this.Resources["UsersDataSource"] as UserCollection;

            _floorsL = new FloorCollection();
            _floorsL = this.Resources["FloorsDataSource"] as FloorCollection;

            _roomL = new RoomCollection();
            _roomL = this.Resources["RoomsDataSource"] as RoomCollection;
        }

        private void ListBox1MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        UserCollection _usersL;
        FloorCollection _floorsL;
        RoomCollection _roomL;
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            
            _usersL.Fill();
            _floorsL.Fill();
            try
            {
                //todo надо брать строку коннекта из конфигов
                string Conn = @"Data Source=microsoft-pc;Initial Catalog=ALFA;Integrated Security=True";
                SqlConnection sqlRoom = new SqlConnection(Conn);
                sqlRoom.Open();
                sqlRoom.Close();
                label12.Content = "Ok";
            }
            catch (Exception)
            {

                label12.Content = "Off";
            }
        }

        private void ButtonDeleteUserClick(object sender, RoutedEventArgs e)
        {
            UsersEnter userSelection = listBox1.SelectedItem as UsersEnter;
            if (userSelection != null)
            {
                AlfaEntities alfaEntities = new AlfaEntities();

                var deleteUser =
                    from useres in alfaEntities.Users
                    where useres.UserId == userSelection.UserId
                    select useres;

                foreach (var user in deleteUser)
                {

                    user.Remove = DateTime.Now;

                }

                try
                {
                    alfaEntities.SaveChanges();
                    _usersL.Remove(userSelection);
                }
                catch (Exception)
                {
                    MessageBox.Show("Не удалось удалить пользователя!");
                }

            }
               
        }

        private void ButtonAddUserClick(object sender, RoutedEventArgs e)
        {
            string username = textBox1.Text;
            string login = textBox2.Text;
            string pass = textBox3.Text;
            string floor = textBox4.Text;
            if (username != null && login != null && pass != null && floor != null)
            {
                AlfaEntities alfaEntities = new AlfaEntities();
                Users users = new Users();
                users.UserName = username.Trim();
                users.Created = DateTime.Now;
                users.Login = login.Trim();
                users.Password = pass.Trim();
                users.GuardOn = true;
                users.KeyOnOff = false;
                users.AlarmOn = true;
                users.DatX = false;
                users.RoomCategories = "1";
                users.CellGroupID = 1;
                users.Type = 1;

                Floors floorses = (from floores in alfaEntities.Floors
                                   where floores.FloorName == floor.Trim()
                                   select floores).FirstOrDefault();
                if (floorses != null)
                {
                    users.FloorId = floorses.FloorId;
                }
                try
                {
                    alfaEntities.Users.AddObject(users);
                    alfaEntities.SaveChanges();
                    _usersL.Clear();
                    _usersL.Fill();
                }
                catch (Exception)
                {

                    MessageBox.Show("Новый пользователь не добавлен. Проверте правильность введенных данных.");
                }
            }
            else
            {
                MessageBox.Show("Введены не все данные для добавления пользователя.");
            }

        }

        private void ButtonTestConnectionClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string ip = textBoxIPBD.Text.Trim();
                string namebd = textBoxNAMEBD.Text.Trim();
                string loginbd = textBoxLOGINBD.Text.Trim();
                string passbd = textBoxPASSBD.Text.Trim();
                string Conn = @"Data Source=" + ip + ";Initial Catalog=" + namebd + "; User ID=" + loginbd + "; Password=" + passbd +"; Integrated Security=True";
                SqlConnection sqlRoom = new SqlConnection(Conn);
                sqlRoom.Open();
                sqlRoom.Close();
                MessageBox.Show("Есть соединение c БД!");
            }
            catch (Exception)
            {
                MessageBox.Show("Cоединение с БД отсутствует, проверте правильность введеных данных!");
            }
        }

        private void ButtonOkConnectionClick(object sender, RoutedEventArgs e)
        {
            string ip = textBoxIPBD.Text.Trim();
            string namebd = textBoxNAMEBD.Text.Trim();
            string loginbd = textBoxLOGINBD.Text.Trim();
            string passbd = textBoxPASSBD.Text.Trim();
            //SetDataBaseConnectionString(namebd, ip, loginbd, passbd);
        }

        private void ListBox2MouseLeftButtonClick(object sender, MouseButtonEventArgs e)
        {
            FloorsEnter floorSelection = listBox2.SelectedItem as FloorsEnter;
            if (floorSelection != null)
            {
                _roomL.Clear();
                _roomL.Fill(floorSelection.FloorId);
            }
        }
    }
}
