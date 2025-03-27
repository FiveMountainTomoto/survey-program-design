using System.IO;
using System.Text;

namespace 矩阵卷积计算
{
    internal class Matrix
    {
        private double[,] _data;
        public int RowCount => _data.GetLength(0);
        public int ColCount => _data.GetLength(1);
        public double this[int row, int col]
        {
            get => _data[row, col]; set => _data[row, col] = value;
        }
        public Matrix(double[,] matrix)
        {
            _data = matrix;
        }
        public Matrix(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            _data = new double[lines.Length, lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] lineSplit = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < lineSplit.Length; j++)
                {
                    _data[i, j] = double.Parse(lineSplit[j]);
                }
            }

        }

        /// <summary>
        /// 打印矩阵
        /// </summary>
        /// <returns></returns>
        public string GetMatrixPrintStr()
        {
            StringBuilder sb = new();
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColCount; j++)
                {
                    sb.Append($"{this[i, j]:F2}   ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
