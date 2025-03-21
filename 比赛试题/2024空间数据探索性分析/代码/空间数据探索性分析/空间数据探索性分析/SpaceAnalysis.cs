using System.IO;
using System.Text;
using static System.Math;

namespace 空间数据探索性分析
{
    internal class SpaceAnalysis
    {
        public List<Event> Events { get; set; }
        public SpaceAnalysis(string dataPath)
        {
            using StreamReader sr = new(dataPath);
            sr.ReadLine();// 跳过第一行

            Events = [];
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) break;

                string[] split = line.Split(",");
                Event ev = new()
                {
                    ID = split[0],
                    X = double.Parse(split[1]),
                    Y = double.Parse(split[2]),
                    AreaCode = int.Parse(split[3])
                };
                Events.Add(ev);
            }
        }

        private List<int>? AreaEventCount;
        private int N = 7; // 分区总数
        private double[,]? w; // 空间权重矩阵
        private double meanCount, S0;// 区域发生事件平均值，全局莫兰指数辅助量
        public string OutputResult()
        {
            StringBuilder sb = new();
            int num = 1;
            Func<string, object, string> outputLine =
                (string explain, object result) => $"{num++}, {explain}, {result}";
            sb.AppendLine("序号, 说明, 结果");

            // 一、读取数据文件
            Event p6 = Events.Single(_ev => _ev.ID == "P6");
            sb.AppendLine(outputLine("P6 的坐标 x", p6.X.ToString("F3")));
            sb.AppendLine(outputLine("P6 的坐标 y", p6.Y.ToString("F3")));
            sb.AppendLine(outputLine("P6 的区号", p6.AreaCode));
            // 1.1 数据统计
            AreaEventCount = Enumerable.Range(1, N).Select(GetAreaEventCount).ToList();
            sb.AppendLine(outputLine("1区（区号为1）的事件数量 n1", AreaEventCount[0]));
            sb.AppendLine(outputLine("4区（区号为4）的事件数量 n4", AreaEventCount[3]));
            sb.AppendLine(outputLine("6区（区号为6）的事件数量 n6", AreaEventCount[5]));
            sb.AppendLine(outputLine("事件总数 n", Events.Count));
            // 1.2 计算平均中心
            (double meanX, double meanY) = GetMeanCenter();
            sb.AppendLine(outputLine("坐标分量 x 的平均值 X", meanX.ToString("F3")));
            sb.AppendLine(outputLine("坐标分量 y 的平均值 Y", meanY.ToString("F3")));
            // 1.3 标准差椭圆计算
            List<double> a = Events.Select(_ev => _ev.X - meanX).ToList();
            List<double> b = Events.Select(_ev => _ev.Y - meanY).ToList();
            double A = Pow(a.Sum(), 2) - Pow(b.Sum(), 2);
            double C = 2 * a.Zip(b, (_ai, _bi) => _ai * _bi).Sum();
            double B = Sqrt(Pow(A, 2) + Pow(C, 2));
            double theta = Atan((A + B) / C);
            double SDEx = Sqrt(2 * a.Zip(b, (_ai, _bi) => Pow((_ai * Cos(theta)) + (_bi * Sin(theta)), 2)).Sum() / Events.Count);
            double SDEy = Sqrt(2 * a.Zip(b, (_ai, _bi) => Pow((_ai * Sin(theta)) - (_bi * Cos(theta)), 2)).Sum() / Events.Count);
            sb.AppendLine(outputLine("P6 坐标分量与平均中心之间的偏移量 a6", a[5].ToString("F3")));
            sb.AppendLine(outputLine("P6 坐标分量与平均中心之间的偏移量 b6", b[5].ToString("F3")));
            sb.AppendLine(outputLine("辅助量 A", A.ToString("F3")));
            sb.AppendLine(outputLine("辅助量 B", B.ToString("F3")));
            sb.AppendLine(outputLine("辅助量 C", C.ToString("F3")));
            sb.AppendLine(outputLine("标准差椭圆长轴与竖直方向的夹角𝜃", theta.ToString("F3")));
            sb.AppendLine(outputLine("标准差椭圆的长半轴𝑆𝐷𝐸𝑥", SDEx.ToString("F3")));
            sb.AppendLine(outputLine("标准差椭圆的长半轴𝑆𝐷𝐸𝑦", SDEy.ToString("F3")));
            // 2.1 计算各区的平均中心
            (double meanX1, double meanY1) = GetMeanCenter(1);
            (double meanX4, double meanY4) = GetMeanCenter(4);
            sb.AppendLine(outputLine("1 区平均中心的坐标分量 X", meanX1.ToString("F3")));
            sb.AppendLine(outputLine("1 区平均中心的坐标分量 Y", meanY1.ToString("F3")));
            sb.AppendLine(outputLine("4 区平均中心的坐标分量 X", meanX4.ToString("F3")));
            sb.AppendLine(outputLine("4 区平均中心的坐标分量 Y", meanY4.ToString("F3")));
            // 2.2 计算各区之间的空间权重矩阵
            w = GetSpacePowerMatrix();
            sb.AppendLine(outputLine(" 1 区和 4 区的空间权重𝑤1,4", w[0, 3].ToString("F6")));
            sb.AppendLine(outputLine(" 6 区和 7 区的空间权重𝑤6,7", w[5, 6].ToString("F6")));
            // 3.1 数据整理
            meanCount = Events.Count / N;
            sb.AppendLine(outputLine("研究区域犯罪事件的平均值𝑋", meanCount.ToString("F6")));
            // 3.2 全局莫兰指数
            S0 = w.Cast<double>().Sum();
            double I = GetMoranI();
            sb.AppendLine(outputLine("全局莫兰指数辅助量S0", S0.ToString("F6")));
            sb.AppendLine(outputLine("全局莫兰指数 I", I.ToString("F6")));
            // 3.3 局部莫兰指数
            List<double> Ii = Enumerable.Range(1, 7).Select(GetMoranI).ToList();
            sb.AppendLine(outputLine("1 区的局部莫兰指数I1", Ii[0].ToString("F6")));
            sb.AppendLine(outputLine("3 区的局部莫兰指数I3", Ii[2].ToString("F6")));
            sb.AppendLine(outputLine("5 区的局部莫兰指数I5", Ii[4].ToString("F6")));
            sb.AppendLine(outputLine("7 区的局部莫兰指数I7", Ii[6].ToString("F6")));
            // 3.4 计算局部莫兰指数的Z得分
            double mu = Ii.Average();
            double sigma = Sqrt(Ii.Select(Iii => Pow(Iii - mu, 2)).Sum() / (N - 1));
            List<double> Z = Ii.Select(Iii => (Iii - mu) / sigma).ToList();
            sb.AppendLine(outputLine("局部莫兰指数的平均数μ", mu.ToString("F6")));
            sb.AppendLine(outputLine("局部莫兰指数的标准差σ", sigma.ToString("F6")));
            sb.AppendLine(outputLine("1 区局部莫兰指数的 Z 得分𝑍1", Z[0].ToString("F6")));
            sb.AppendLine(outputLine("3 区局部莫兰指数的 Z 得分𝑍3", Z[2].ToString("F6")));
            sb.AppendLine(outputLine("5 区局部莫兰指数的 Z 得分𝑍5", Z[4].ToString("F6")));
            sb.AppendLine(outputLine("7 区局部莫兰指数的 Z 得分𝑍7", Z[6].ToString("F6")));
            return sb.ToString();
        }

        /// <summary>
        /// 统计区域事件数量，用于 1.1 数据统计
        /// </summary>
        /// <param name="areaCode">区号</param>
        /// <returns>区域事件数量</returns>
        private int GetAreaEventCount(int areaCode)
        {
            return Events.Count(_ev => _ev.AreaCode == areaCode);
        }

        /// <summary>
        /// 计算所有事件点空间位置的算术平均值，用于 1.2 计算平均中心
        /// </summary>
        /// <returns>(x坐标平均值, y坐标平均值)</returns>
        private (double, double) GetMeanCenter()
        {
            return (Events.Average(_ev => _ev.X), Events.Average(_ev => _ev.Y));
        }

        /// <summary>
        /// 计算某区所有事件点空间位置的算术平均值，用于 2.1 计算各区的平均中心
        /// </summary>
        /// <param name="areaCode">区号</param>
        /// <returns>(x坐标平均值, y坐标平均值)</returns>
        private (double, double) GetMeanCenter(int areaCode)
        {
            IEnumerable<Event> evs = Events.Where(_ev => _ev.AreaCode == areaCode);
            double x = evs.Average(_ev => _ev.X);
            double y = evs.Average(_ev => _ev.Y);
            return (x, y);
        }

        /// <summary>
        /// 计算两个区之间的权，用于 2.2 计算各区之间的空间权重矩阵
        /// </summary>
        /// <param name="areaCode1">区号1</param>
        /// <param name="areaCode2">区号2</param>
        /// <returns>两个区之间的权</returns>
        private double GetPower(int areaCode1, int areaCode2)
        {
            if (areaCode1 == areaCode2) return 0;
            (double meanX1, double meanY1) = GetMeanCenter(areaCode1);
            (double meanX2, double meanY2) = GetMeanCenter(areaCode2);
            double d = Sqrt(Pow(meanX1 - meanX2, 2) + Pow(meanY1 - meanY2, 2));
            return 1000 / d;
        }

        /// <summary>
        /// 计算权阵，用于 2.2 计算各区之间的空间权重矩阵
        /// </summary>
        /// <returns>密集的空间权重矩阵</returns>
        private double[,] GetSpacePowerMatrix()
        {
            double[,] powMat = new double[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    powMat[i, j] = GetPower(i + 1, j + 1);
                }
            }
            // 非要用LINQ也可以，返回一个交错数组double[][]，并不优雅，不建议
            //return Enumerable.Range(0, N)
            //    .Select(i => Enumerable.Range(0, N)
            //                           .Select(j => GetPower(i + 1, j + 1)).ToArray())
            //    .ToArray()
            return powMat;
        }

        /// <summary>
        /// 计算全局莫兰指数，用于 3.2 全局莫兰指数
        /// </summary>
        /// <returns>全局莫兰指数</returns>
        private double GetMoranI()
        {
            double numerator1 = 0;// 分子
            double dominator1 = 0;// 分母
            for (int i = 0; i < N; i++)
            {
                double xi = AreaEventCount![i];
                dominator1 += Pow(xi - meanCount, 2);
                for (int j = 0; j < N; j++)
                {
                    double xj = AreaEventCount[j];
                    numerator1 += w![i, j] * (xi - meanCount) * (xj - meanCount);
                }
            }
            return N * numerator1 / S0 / dominator1;
            // 当然可以用LINQ一行搞定，不建议
            //return N
            //    * Enumerable.Range(0, N).Select(i => Enumerable.Range(0, N).Select(j => w[i, j] * (AreaEventCount[i] - meanCount) * (AreaEventCount[j] - meanCount)).Sum()).Sum()
            //    / S0
            //    / Enumerable.Range(0, N).Select(i => Pow(AreaEventCount[i] - meanCount, 2)).Sum();
        }

        /// <summary>
        /// 计算局部莫兰指数，用于 3.3 局部莫兰指数
        /// </summary>
        /// <param name="areaCode">区号</param>
        /// <returns>局部莫兰指数</returns>
        private double GetMoranI(int areaCode)
        {
            int i = areaCode - 1;
            double xi = AreaEventCount![i];
            double tmp1 = 0, Si2 = 0;
            for (int j = 0; j < N; j++)
            {
                if (j == i) continue;
                double xj = AreaEventCount[j];
                tmp1 += w![i, j] * (xj - meanCount);
                Si2 += Pow(xj - meanCount, 2);
            }
            Si2 /= N - 1;
            return (xi - meanCount) * tmp1 / Si2;
        }
    }
}
