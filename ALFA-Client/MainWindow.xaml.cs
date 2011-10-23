using System.Windows;
using System.Linq;
using ALFA_Client.Entities;
using NLog;

namespace ALFA_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
 
        }

        private void LogInClick(object sender, RoutedEventArgs e)
        {
            Enter(logintext.Text, passtext.Password);
        }

        private Logger _logger = LogManager.GetCurrentClassLogger();

        private void Enter(string login, string password)
        {
            AlfaEntities alfaEntities = new AlfaEntities();

            if (login.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Поля Логин или Пароль не должны быть пустыми. Введите логин и пароль.");
            }
            else
            {
                 Users userItem =   (from userse in alfaEntities.Users
                                    where ((userse.Login == login) && (userse.Password == password) && (userse.Remove == null))
                                    select userse).FirstOrDefault();

                if (userItem != null)
                {
                    bool allOk = true;
                    string username;
                    bool guardOn;
                    string roomCateory;
                    bool alarmOn;
                    bool keyOnOff;
                    bool datX;
                    int cellgroupId;
                    int floorId = 0;
                    int type;


                    username = userItem.UserName;

                    if (userItem.GuardOn != null)
                    {
                        guardOn = (bool)userItem.GuardOn;
                    }
                    else
                    {
                        guardOn = false;
                    }

                    //todo wtf почему стояло условие != "" а описание "Не указан CatRoom." противоречие пля
                    roomCateory = userItem.RoomCategories;
                    if (roomCateory == "")
                    {
                        allOk = false;
                        MessageBox.Show("Не указан CatRoom.");
                    }

                    if (userItem.AlarmOn != null)
                    {
                        alarmOn = (bool) userItem.AlarmOn;
                    }
                    else
                    {
                        alarmOn = false;
                    }

                    if (userItem.KeyOnOff != null)
                    {
                        keyOnOff = (bool) userItem.KeyOnOff;
                    }
                    else
                    {
                        keyOnOff = false;
                    }

                    if (userItem.DatX != null)
                    {
                        datX = (bool) userItem.DatX;
                    }
                    else
                    {
                        datX = false;
                        allOk = false;
                        MessageBox.Show("Не указан DatX.");
                    }

                    if (userItem.CellGroupID != null)
                    {
                        cellgroupId = (int)userItem.CellGroupID;
                    }
                    else
                    {
                        cellgroupId = 1;
                        allOk = false;
                        MessageBox.Show("Не указан CellgroupID.");
                    }

                    if (userItem.Type != null)
                    {
                        type = (int)userItem.Type;
                    }
                    else
                    {
                        type = 1;
                        allOk = false;
                        MessageBox.Show("Не указан тип пользователя.");
                    }

                    //todo нифига не понял что там с этажами
                    if (userItem.FloorId != null)
                    {
                        floorId = (int) userItem.FloorId;
                    }
                    else
                    {
                        allOk = false;
                        //todo wtf не понятное описание, на сколько я понимаю если нет ни одного этажа, то фраза должна быть 
                        //todo "пользователь не имеет права на просмотр ни одного этажа" ну или ченить подобное
                        //todo сделать связь многие ко многим
                        //MessageBox.Show("Отсутствуют права на просмотр какого-либо этажа.");
                        MessageBox.Show("пользователь не имеет права на просмотр ни одного этажа");
                    }

                    if (allOk == true && type == 2)
                    {
                        Admin admin = new Admin(username);
                        this.Hide();
                        admin.Show();
                        this.Close();
                    }

                    if (allOk == true && type == 1)
                    {
                        UserWindow userWindow = new UserWindow(username, guardOn, alarmOn, keyOnOff, datX, cellgroupId, roomCateory, floorId);
                        this.Hide();
                        userWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("не правильно введено имя пользователя или пароль");
                }

            }
        }
    }
}
