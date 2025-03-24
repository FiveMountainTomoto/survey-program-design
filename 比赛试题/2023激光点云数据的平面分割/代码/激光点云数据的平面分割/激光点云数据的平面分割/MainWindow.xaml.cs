using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 激光点云数据的平面分割
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PointDataHandler? pdh;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadingButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfdl = new()
            {
                Title = "读取数据",
                Filter = "数据文件(*.txt)|*.txt"
            };
            if (opfdl.ShowDialog() == true)
            {
                pdh = new(opfdl.FileName);
                dataTextBox.Text = File.ReadAllText(opfdl.FileName);
                tabCtrl.SelectedIndex = 0;
            }
        }

        private void calButton_Click(object sender, RoutedEventArgs e)
        {
            if(pdh is null)
            {
                MessageBox.Show("请先读取数据");
                return;
            }
            resultTextBox.Text = pdh.OutputResult();
            tabCtrl.SelectedIndex = 1;
        }

        private void outputButton_Click(object sender, RoutedEventArgs e)
        {
            if (pdh is null)
            {
                MessageBox.Show("请先读取数据");
                return;
            }
            if (pdh.GetPoiDivideResult(out string? result))
            {
                SaveFileDialog sfd = new()
                {
                    Title = "保存结果",
                    Filter = "文本文档(*.txt)|*.txt"
                };
                if (sfd.ShowDialog() == true)
                {
                    File.WriteAllText(sfd.FileName, result);
                }
            }
            else
            {
                MessageBox.Show("请先进行计算");
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new()
            {
                Title = "保存运行结果",
                Filter = "文本文档(*.txt)|*.txt"
            };
            if (sfd.ShowDialog() == true)
            {
                File.WriteAllText(sfd.FileName, resultTextBox.Text);
            }
        }
    }
}