using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestANN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            AudioPlayback a = new AudioPlayback();
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                //WaveGenerator wg = new WaveGenerator(WaveExampleType.ExampleSineWave);
                //wg.Read(dlg.FileName);
                //wg.Save(dlg.FileName+"2");
                a.Load(dlg.FileName);
                a.Play();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == true)
            {
                WaveGenerator wg = new WaveGenerator(WaveExampleType.ExampleSineWave);
                wg.Save(dlg.FileName);
            }

        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {
            Polyline pl = new Polyline();
            pl.Stroke = new SolidColorBrush(Colors.Black);
            pl.Points.Add(new Point(10, 10));
            pl.Points.Add(new Point(100, 100));
            MyCanvas.Children.Add(pl);
            MyCanvas.UpdateLayout();
        }
    }
}
