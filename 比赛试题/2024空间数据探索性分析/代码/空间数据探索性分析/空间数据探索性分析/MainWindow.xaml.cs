using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;

namespace 空间数据探索性分析
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SpaceAnalysis? spaAna;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void readingButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfdl = new()
            {
                Title = "选择数据文件",
                Filter = "数据文件(*.txt)|*.txt",
            };
            if (opfdl.ShowDialog() == true)
            {
                spaAna = new(opfdl.FileName);
                // 打印数据
                dataTextBox.Text = "id, x(m), y(m), area_code\n";
                StringBuilder sb = new();
                foreach (Event ev in spaAna.Events)
                {
                    sb.AppendLine($"{ev.ID}, {ev.X:F3}, {ev.Y:F3}, {ev.AreaCode}");
                }
                dataTextBox.AppendText(sb.ToString());
            }
        }

        private void calculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (spaAna is null)
            {
                MessageBox.Show("请先读取数据");
                return;
            }

            resultTextBox.Text = spaAna.OutputResult();
            tabCtrl.SelectedIndex = 1;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfdl = new()
            {
                Title = "保存运行结果",
                Filter = "文本文档(*.txt)|*.txt",
            };
            if (sfdl.ShowDialog() == true)
            {
                File.WriteAllText(sfdl.FileName, resultTextBox.Text);
                MessageBox.Show("保存成功！");
            }
        }
    }
}