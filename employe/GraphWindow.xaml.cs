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
    public partial class GraphWindow : Window
    {
        private Canvas canvas;

        public GraphWindow()
        {
            InitializeComponent();

            // Создание нового Canvas
            canvas = new Canvas();
            canvas.Width = 50;
            canvas.Height = 25;

            // Добавление Canvas как содержимого окна
            this.Content = canvas;
            DrawFunction();
        }

        private void DrawFunction()
        {
            Polyline polyline = new Polyline();
            polyline.Stroke = Brushes.Green;
            polyline.StrokeThickness = 2;


            double x = -2.0;
            while (x <= 2.0)
            {
                double y = x * x;
                Point point = new Point((x + 2.0) * 20, 40 - y * 20);
                polyline.Points.Add(point);
                x += 0.2;
            }

            canvas.Children.Add(polyline);
        }

        public void btnGoToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }
    }
}
