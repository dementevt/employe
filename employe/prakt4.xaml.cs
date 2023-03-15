using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace employe
{
    /// <summary>
    /// Логика взаимодействия для prakt4.xaml
    /// </summary>
    public partial class prakt4 : Window
    {
        public prakt4()
        {
            InitializeComponent();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        //Служит для перемещения окна за заголовок формы
        private void TextBlock_MouseLeftButtonDown(object sender,
        MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
