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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Timer idleTimer; // таймер простоя
        private const int idleTime = 60; // время простоя в секундах
        //private Canvas canvas;

        public MainWindow()
        {
            InitializeComponent();

            idleTimer = new Timer(idleTime * 1000);
            idleTimer.Elapsed += IdleTimerElapsed;
            idleTimer.AutoReset = true;
            idleTimer.Start();
            /*
            // Создание нового Canvas
            canvas = new Canvas();
            canvas.Width = 50;
            canvas.Height = 25;

            // Добавление Canvas как содержимого окна
            //this.Content = canvas;
            DrawFunction();*/
        }

        private void IdleTimerElapsed([DisallowNull] object sender, ElapsedEventArgs e)
        {
            // Вызываем диалоговое окно напоминания о неиспользовании приложения
            Dispatcher.Invoke(() => MessageBox.Show("Внимание! Вы неактивны более одной " +
                "минуты. Вы можете закрыть приложение.", "Напоминание",
                MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Обнуляем таймер простоя
            idleTimer.Stop();
            bool isMainWindowOpen = false;

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow && window.Name == "mainWindow")
                {
                    isMainWindowOpen = true;
                    break;
                }
            }
            if (isMainWindowOpen)
            {
                idleTimer.Start();
            }
            else
            {
                idleTimer.Stop();
            }
        }
        public List<string> ReadEmployeeIdsFromFile()
        {
            List<string> ids = new List<string>();
            using (StreamReader sr = new StreamReader("Z:\\Documents\\thirdCourse\\Практика_1\\employe\\employe\\employe.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] fields = line.Split('\t');
                    string id = fields[0].Trim();
                    ids.Add(id);
                }
            }
            return ids;
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            // Получаем значения всех полей
            string id = idTextBox.Text;
            string lastName = lastNameTextBox.Text;
            string firstName = firstNameTextBox.Text;
            string middleName = middleNameTextBox.Text;
            string passport = passportTextBox.Text;
            string phoneNumber = phoneNumberTextBox.Text;
            string email = emailTextBox.Text;
            string password = passwordTextBox.Text;


            // Получаем все существующие идентификаторы из файла
            List<string> employesIds = ReadEmployeeIdsFromFile();

            if (CheckValidity(id, lastName, firstName, middleName, passport, phoneNumber, email, employesIds, password))
            {
                // Сохраняем данные в файл "employee.txt"
                using (StreamWriter sw = new StreamWriter("Z:\\Documents\\thirdCourse\\Практика_1\\" +
                    "employe\\employe\\employe.txt", true))
                {
                    sw.WriteLine($"{id} \t{lastName} \t{firstName} \t{middleName} " +
                        $"\t{passport} \t{phoneNumber} \t{email}");
                    
                }

                // Сохраняем id и пароль в файл "password.txt"
                using (StreamWriter sw = new StreamWriter("Z:\\Documents\\thirdCourse\\Практика_1\\" +
                    "employe\\employe\\password.txt", true))
                {
                    sw.Write($"\n{id},{password}");

                }

                // Очищаем все поля
                idTextBox.Text = "";
                lastNameTextBox.Text = "";
                firstNameTextBox.Text = "";
                middleNameTextBox.Text = "";
                passportTextBox.Text = "";
                phoneNumberTextBox.Text = "";
                emailTextBox.Text = "";
                passwordTextBox.Text = "";

                // Показываем сообщение об успешном добавлении сотрудника
                MessageBox.Show("Вы успешно зарегистрировали сотрудника.", "Успешно",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private bool CheckValidity(string id, string lastName, string firstName, string middleName, string passport, string phoneNumber, string email, List<string> employeeIds, string password)
        {
            //Проверяем, что все обязательные поля заполнены
            string emptyFields = string.Empty;

            if (string.IsNullOrEmpty(id))
                emptyFields += "id, ";
            if (string.IsNullOrEmpty(lastName))
                emptyFields += "lastName, ";
            if (string.IsNullOrEmpty(firstName))
                emptyFields += "firstName, ";
            if (string.IsNullOrEmpty(passport))
                emptyFields += "passport, ";
            if (string.IsNullOrEmpty(email))
                emptyFields += "email, ";
            if (string.IsNullOrEmpty(phoneNumber))
                emptyFields += "phoneNumber, ";
            if (string.IsNullOrEmpty(password))
                emptyFields += "password, ";

            if (!string.IsNullOrEmpty(emptyFields))
            {
                emptyFields = emptyFields.TrimEnd(' ', ',');
                MessageBox.Show($"Поля {emptyFields} не заполнены!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            // Проверка серии и номера паспорта 
            string patternPassport = @"^\d{4}\s\d{6}$";

            if (!Regex.IsMatch(passport, patternPassport))
            {
                MessageBox.Show("Серия и номер паспорта неправильного формата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка индентификатора
            string patternId = @"^\d+$";

            if (!Regex.IsMatch(id, patternId))
            {
                MessageBox.Show("Идентификатор неправильного формата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (employeeIds.Contains(id))
            {
                MessageBox.Show($"Не удалось добавить сотрудника с идентификатором {id}, такой идентификатор уже использован", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка ФИО
            string patternFIO = @"^[А-ЯЁ][а-яё]*$";

            if (!Regex.IsMatch(lastName, patternFIO))
            {
                MessageBox.Show("Фамилия неправильного формата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!Regex.IsMatch(firstName, patternFIO))
            {
                MessageBox.Show("Имя неправильного формата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Можно не проверять, если поле не является обязательным
            if (!string.IsNullOrEmpty(middleName) && !Regex.IsMatch(middleName, patternFIO))
            {
                MessageBox.Show("Отчество неправильного формата", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка номера телефона
            string patternPhone = @"^(\+7|8)[\- ]?\(?\d{3}\)?[\- ]?\d{3}[\- ]?\d{2}[\- ]?\d{2}$";
            
            if (!string.IsNullOrEmpty(phoneNumber) && !Regex.IsMatch(phoneNumber, patternPhone))
            {
                MessageBox.Show("Телефонный номер неправильного формата.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            //Проверка корректности email
            //паттерн регулярного выражения для проверки email адреса
            string patternEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, patternEmail))
            {
                MessageBox.Show("Неправильный формат email адреса.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }


        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {

            AuthWindow authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
            idleTimer.Stop();

        }

        private void GoToGraphWindow_Click(object sender, RoutedEventArgs e)
        {
            GraphWindow graphWindow = new();
            graphWindow.Show();
            this.Close();

            idleTimer.Stop();
        }


        /*
        private void DrawFunction()
        {
            Polyline polyline = new Polyline();
            polyline.Stroke = Brushes.Blue;
            polyline.StrokeThickness = 2;


            double x = -2.0;
            while (x <= 2.0)
            {
                double y = x * x;
                Point point = new Point((x + 2.0) * 10, 20 - y * 10);
                polyline.Points.Add(point);
                x += 0.1;
            }

            canvas.Children.Add(polyline);
        }*/

    }
}

