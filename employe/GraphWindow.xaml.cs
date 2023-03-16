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
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;

namespace employe
{
    public partial class GraphWindow : Window
    {

        const int countDot = 170;
        List<double[]> dataList = new();
        DrawingGroup drawingGroup = new();
        public GraphWindow()
        { 
            InitializeComponent();

            DataFill(); 
            Execute();  
            image1.Source = new DrawingImage(drawingGroup);
        }

        void DataFill()
        {
            double[] sin = new double[countDot + 1];

            for(int i = 0; i < sin.Length; i++) 
            {
                double angle = ((Math.PI / 2) / countDot) * i;
                sin[i] =  Math.Pow((Math.Cos(Math.Pow((Math.Pow(angle, 3) + 2), 2))), 3);
            }
            dataList.Add(sin);

        }

        private void BackgroundFun()
        {
            GeometryDrawing geometryDrawing = new();

            RectangleGeometry rectGeometry = new()
            {
                Rect = new Rect(0, 0, 1, 1)
            };
            geometryDrawing.Geometry = rectGeometry;

            geometryDrawing.Pen = new Pen(Brushes.Blue, 0.005);
            geometryDrawing.Brush = Brushes.LightBlue;

            drawingGroup.Children.Add(geometryDrawing);

        }

        
        private void GridFun()
        {
            GeometryGroup geometryGroup = new();

            // Создаем и добавляем в коллекцию десять параллельных линий
            for (int i = 1; i < 10; i++)
            {
                LineGeometry line = new(new Point(1.0, i * 0.1), new Point(-0.1, i * 0.1));
                geometryGroup.Children.Add(line);
            }

            // Сохраняем описание геометрии
            GeometryDrawing geometryDrawing = new()
            {
                Geometry = geometryGroup,

                // Настраиваем перо
                Pen = new Pen(Brushes.Gray, 0.003)
            };
            double[] dashes = { 1, 1, 1, 1, 1 };// Образец штриха
            geometryDrawing.Pen.DashStyle = new DashStyle(dashes, -.1);
            geometryDrawing.Brush = Brushes.Beige;

            // Добавляем готовый слой в контейнер отображения
            drawingGroup.Children.Add(geometryDrawing);
            
        }
        private void SinFun()
        {
            GeometryGroup geometryGroup = new ();
            for (int i = 0; i < dataList[0].Length - 1; i++)
            {
                LineGeometry line = new(
                    new Point((double)i / (double)countDot,
                    .5 - (dataList[0][i] / 2.0)),
                    new Point((double)(i + 1) / (double)countDot,
                    .5 - (dataList[0][i + 1] / 2.0)));
                geometryGroup.Children.Add(line);

            }

            // Сохраняем описание геометрии
            GeometryDrawing geometryDrawing = new()
            {
                Geometry = geometryGroup,
                // Настраиваем перо
                Pen = new Pen(Brushes.Green, 0.01)
            };
            drawingGroup.Children.Add(geometryDrawing);
        }

        //private void CosFun()
        //{
        //    GeometryGroup geometryGroup = new();
        //    for (int i = 0; i < dataList[1].Length; i++)
        //    {
        //        EllipseGeometry ellips = new(
        //            new Point((double)i / (double)countDot,
        //            .5 - (dataList[1][i] / 2.0)), 0.01, 0.01);
        //        geometryGroup.Children.Add(ellips);
        //    }

        //    //Сохраняем описание геометрии
        //    GeometryDrawing geometryDrawing = new();
        //    geometryDrawing.Geometry = geometryGroup;

        //    // Настраиваем перо
        //    geometryDrawing.Pen = new Pen(Brushes.Red, 0.01);

        //    //Добавляем готовый слой в контейнер отображения
        //    drawingGroup.Children.Add(geometryDrawing);
        //}

        private void MarkerFun()
        {
            GeometryGroup geometryGroup = new();
            for (int i = 0; i <= 10; i++)
            {
                FormattedText formattedText = new FormattedText(
                     String.Format("{0,7:F}", 1 - i * 0.2),
                     CultureInfo.InvariantCulture,
                     FlowDirection.LeftToRight,
                     new Typeface("Verdana"),
                     0.05,
                     Brushes.Black);
                formattedText.SetFontWeight(FontWeights.Bold);
                Geometry geometry = formattedText.BuildGeometry(new Point(-0.2, i * 0.1 - 0.03));
                geometryGroup.Children.Add(geometry);
            }

            GeometryDrawing geometryDrawing = new()
            {
                Geometry = geometryGroup,
                Brush = Brushes.LightGray,
                Pen = new Pen(Brushes.Gray, 0.003)
            };
            drawingGroup.Children.Add(geometryDrawing);
        }

        void Execute()
        {
            BackgroundFun();
            GridFun();
            SinFun();
            //CosFun();
            MarkerFun();
        }

        public void btnGoToMainWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }
    }
}
