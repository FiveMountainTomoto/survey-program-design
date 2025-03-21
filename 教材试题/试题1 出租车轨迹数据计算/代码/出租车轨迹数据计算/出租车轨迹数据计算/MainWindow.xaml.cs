using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 出租车轨迹数据计算
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaxiDataHandler? handler;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loadingButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfdl = new()
            {
                Title = "读取数据",
                Filter = "数据文件(*.txt)|*.txt",
            };
            if (opfdl.ShowDialog() == true)
            {
                handler = new(opfdl.FileName);
                dataTextBox.Text = File.ReadAllText(opfdl.FileName);
                tabCtrl.SelectedIndex = 0;
            }
        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (handler is null)
            {
                MessageBox.Show("请先导入数据");
                return;
            }
            resultTextBox.Text = handler.OutputResult();
            tabCtrl.SelectedIndex = 1;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(resultTextBox.Text))
            {
                MessageBox.Show("请先进行计算");
                return;
            }
            SaveFileDialog sfdl = new()
            {
                Title = "保存结果",
                Filter = "文本文档(*.txt)|*.txt",
            };
            if (sfdl.ShowDialog() == true)
            {
                File.WriteAllText(sfdl.FileName, resultTextBox.Text);
            }
        }
    }
}