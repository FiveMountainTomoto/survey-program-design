using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace 矩阵卷积计算;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Matrix? M, N;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void loadMButton_Click(object sender, RoutedEventArgs e)
    {
        if (TryGetFilePath(out string? filePath))
        {
            M = new(filePath!);
            dataTextBox.AppendText($"矩阵M：\n{M.GetMatrixPrintStr()}");
            tabCtrl.SelectedIndex = 0;
        }
    }

    private bool TryGetFilePath(out string? filePath)
    {
        OpenFileDialog opfdl = new()
        {
            Title = "读取数据",
            Filter = "数据文件(*.txt)|*.txt"
        };
        if (opfdl.ShowDialog() == true)
        {
            filePath = opfdl.FileName;
            return true;
        }
        else
        {
            filePath = null;
            return false;
        }
    }

    private void loadNButton_Click(object sender, RoutedEventArgs e)
    {
        if (TryGetFilePath(out string? filePath))
        {
            N = new(filePath!);
            dataTextBox.AppendText($"矩阵N：\n{N.GetMatrixPrintStr()}");
            tabCtrl.SelectedIndex = 0;
        }
    }

    private void saveButton_Click(object sender, RoutedEventArgs e)
    {
        SaveFileDialog sfdl = new()
        {
            Title = "保存结果",
            Filter = "文本文档(*.txt)|*.txt"
        };
        if (sfdl.ShowDialog() == true)
        {
            File.WriteAllText(sfdl.FileName, resultTextBox.Text);
        }
        MessageBox.Show("保存成功！");
    }

    private void calButton_Click(object sender, RoutedEventArgs e)
    {
        if (M is null)
        {
            MessageBox.Show("请先读取矩阵M");
            return;
        }
        if (N is null)
        {
            {
                MessageBox.Show("请先读取矩阵N");
                return;
            }
        }
        MatrixConvHandler mch = new(M, N);
        mch.Calculate(out Matrix res1, out Matrix res2);
        resultTextBox.Text = $"算法1结果\n{res1.GetMatrixPrintStr()}算法2结果\n{res2.GetMatrixPrintStr()}";
        tabCtrl.SelectedIndex = 1;
    }
}