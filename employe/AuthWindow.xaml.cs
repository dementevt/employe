using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Threading;
using System.Timers;
using System.Diagnostics.CodeAnalysis;

namespace employe
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private Timer idleTimer2; // таймер простоя
        private const int idleTime2 = 60; // время простоя в секундах

        public AuthWindow()
        {
            InitializeComponent(); 

            idleTimer2 = new Timer(idleTime2 * 1000);
            idleTimer2.Elapsed += IdleTimerElapsed;
            idleTimer2.AutoReset = true;
            idleTimer2.Start();
        }
        //
        private void IdleTimerElapsed([DisallowNull] object sender2, ElapsedEventArgs e)
        {
                // Вызываем диалоговое окно напоминания о неиспользовании приложения
                Dispatcher.Invoke(() => MessageBox.Show("Внимание! Вы неактивны более одной " +
                    "минуты. Вы можете закрыть приложение.", "Напоминание",
                    MessageBoxButton.OK, MessageBoxImage.Information));
           
        }

        private void AuthWindow_MouseMove(object sender2, System.Windows.Input.MouseEventArgs e)
        {
            // Обнуляем таймер простоя
            idleTimer2.Stop();
            bool isAuthWindowOpen = false;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is AuthWindow && window.Name == "authWindow")
                {
                    isAuthWindowOpen = true;
                    break;
                }
            }
            if (isAuthWindowOpen)
            {
                idleTimer2.Start();
            }
            else
            {
                idleTimer2.Stop();
            }

        }

        private int remainingAttempts = 3;
        private DateTime? lockoutEndTime = null;
        private DateTime lastFailedAttemptTime = DateTime.MinValue;

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что все обязательные поля заполнены
            if (string.IsNullOrEmpty(idTextBox.Text) || string.IsNullOrEmpty(passwordTextBox.Password))
            {
                // Выводим сообщение об ошибке
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Проверяем правильность введенных данных и авторизуем сотрудника
            bool isAuthorized = CheckAuthorization(idTextBox.Text, passwordTextBox.Password);
            if (isAuthorized)
            {
                //Останвливаем таймер простоя
                idleTimer2.Stop();
                MessageBox.Show($"Вы успешно вошли!");
                this.Close();
                GraphWindow graphWindow = new ();
                graphWindow.Show();
                graphWindow.Close();
            }
            else
            {
                // Очищаем все поля
                idTextBox.Text = "";
                passwordTextBox.Password = "";

                //Защита от подбора пароля
              
                    remainingAttempts--;

                    if (remainingAttempts == 0)
                    {
                        lastFailedAttemptTime = DateTime.Now;
                        //lockoutEndTime = DateTime.Now.AddSeconds(60);
                        MessageBox.Show($"Вы исчерпали все попытки входа. Попробуйте снова через 1 минуту.");
                    }
                    else
                    {
                        // Выводим сообщение об ошибке
                        MessageBox.Show($"Неправильный логин или пароль. У вас осталось {remainingAttempts} попыток.");
                    }
                
               /* else
                {
                    MessageBox.Show("Вы успешно вошли в систему!");
                    remainingAttempts = 3;
                }*/

                // Выводим сообщение об ошибке
                //MessageBox.Show("Неправильный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                 
                //Блокируем ввод данных
                if (DateTime.Now - lastFailedAttemptTime < TimeSpan.FromMinutes(1))
                 {
                    // Если прошло меньше минуты, то блокируем ввод и выходим из метода
                    passwordTextBox.IsEnabled = false;
                    var timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(60);
                    timer.Tick += (sender, args) =>
                    {
                        passwordTextBox.IsEnabled = true;
                        idTextBox.IsEnabled = true;
                        btnLogin.IsEnabled= true;   
                        timer.Stop();
                    };
                    timer.Start();
                    return;
                 }
            }
        }
        /*
        public List<string> ReadEmployeePasswordFromFile()
        {
            List<string> passwords = new List<string>();
            using (StreamReader sr = new StreamReader("Z:\\Documents\\thirdCourse\\Практика_1\\" +
                "employe\\employe\\password.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] fields = line.Split('\n');
                    string id = fields[0].Trim();
                    passwords.Add(id);
                }
            }
            return passwords;
        }

        private bool CheckAuthorization(string login, string password)
        {
            
            string userLogin = "admin";
            string userPassword = "0000";

            List<string> employesPasswords = ReadEmployeePasswordFromFile();
            foreach(string )
            if (login == userLogin && password == userPassword)
            {
                return true;
            }
            else
            {
                return false;
            }

            
        }*/

        public Dictionary<string, string> ReadEmployeeIdAndPasswordFromFile()
        {
            Dictionary<string, string> passwords = new Dictionary<string, string>();
            using (StreamReader sr = new StreamReader("Z:\\Documents\\thirdCourse\\Практика_1\\" +
                "employe\\employe\\password.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] fields = line.Split(',');
                    if (fields.Length >= 2)
                    {
                        string login = fields[0].Trim();
                        string password = fields[1].Trim();
                        passwords.Add(login, password);
                    }
                }
            }
            return passwords;
        }

        private bool CheckAuthorization(string login, string password)
        {
            Dictionary<string, string> employees = ReadEmployeeIdAndPasswordFromFile();
            if (employees.ContainsKey(login) && employees[password] == password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void btnGoToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
            idleTimer2.Stop();
        }
    }
}
