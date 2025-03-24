using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using 激光点云数据的平面分割.DataStruct;
using static System.Math;

namespace 激光点云数据的平面分割
{
    public class PointDataHandler
    {
        public List<Point> Points { get; private set; }
        public PointDataHandler(string dataPath)
        {
            Points = LoadingData(dataPath);
        }

        public string OutputResult()
        {
            StringBuilder sb = new();
            int num = 1;
            Action<string, object> appendLine = (explain, value) =>
            {
                sb.AppendLine($"{num++}, {explain}, {value}");
            };
            // 一、数据文件读取
            Point p5 = Points[4];
            appendLine("P5 的坐标分量 x", p5.X.ToString("F3"));
            appendLine("P5 的坐标分量 y", p5.Y.ToString("F3"));
            appendLine("P5 的坐标分量 z", p5.Z.ToString("F3"));
            appendLine("坐标分量 x 的最小值 xmin", Points.Min(p => p.X).ToString("F3"));
            appendLine("坐标分量 x 的最大值 xmax", Points.Max(p => p.X).ToString("F3"));
            appendLine("坐标分量 y 的最小值 ymin", Points.Min(p => p.Y).ToString("F3"));
            appendLine("坐标分量 y 的最大值 ymax", Points.Max(p => p.Y).ToString("F3"));
            appendLine("坐标分量 z 的最小值 zmin", Points.Min(p => p.Z).ToString("F3"));
            appendLine("坐标分量 z 的最大值 zmax", Points.Max(p => p.Z).ToString("F3"));
            // 1.1 点云数据栅格化，1.2 计算栅格单元的几何特征信息
            int i = Grid.GetPoiGridRow(p5), j = Grid.GetPoiGridCol(p5);
            Grid C = new(2, 3, Points);
            appendLine("P5 点的所在栅格的行 i", i);
            appendLine("P5 点的所在栅格的列 j", j);
            appendLine("栅格 C 中的点的数量", C.PoiCount);
            appendLine("栅格 C 中的平均高度", C.HeightAverage.ToString("F3"));
            appendLine("栅格 C 中高度的最大值", C.HeightMax.ToString("F3"));
            appendLine("栅格 C 中的高度差", C.HeightDiff.ToString("F3"));
            appendLine("栅格 C 中的高度方差", C.HeightVar.ToString("F3"));
            // 2.1 平面拟合
            Plane S1 = new(Points[0], Points[1], Points[2], Points);
            appendLine("P1-P2-P3 构成三角形的面积", GetTriangleArea(Points[0], Points[1], Points[2]).ToString("F6"));
            appendLine("拟合平面 S1 的参数 A", S1.A.ToString("F6"));
            appendLine("拟合平面 S1 的参数 B", S1.B.ToString("F6"));
            appendLine("拟合平面 S1 的参数 C", S1.C.ToString("F6"));
            appendLine("拟合平面 S1 的参数 D", S1.D.ToString("F6"));
            // 2.2 内部点和外部点计算
            appendLine("P1000 到拟合平面 S1 的距离", S1.GetDistance(Points[999]).ToString("F3"));
            appendLine("P5 到拟合平面 S1 的距离", S1.GetDistance(Points[4]).ToString("F3"));
            appendLine("拟合平面 S1 的内部点数量", S1.InternalPoiCount);
            appendLine("拟合平面 S1 的外部点数量", S1.ExternalPoiCount);
            // 2.3 最佳分割平面计算
            Plane J1 = GetBestPlane(Points, 300);
            appendLine("最佳分割平面 J1 的参数 A", J1.A.ToString("F6"));
            appendLine("最佳分割平面 J1 的参数 B", J1.B.ToString("F6"));
            appendLine("最佳分割平面 J1 的参数 C", J1.C.ToString("F6"));
            appendLine("最佳分割平面 J1 的参数 D", J1.D.ToString("F6"));
            appendLine("最佳分割平面 J1 的内部点数量", J1.InternalPoiCount);
            appendLine("最佳分割平面 J1 的外部点数量", J1.ExternalPoiCount);
            // 2.4 迭代计算平面分割
            List<Point> poisExceptJ1 = Points.Except(J1.FittingPoints).Except(J1.InternalPoints).ToList();
            Plane J2 = GetBestPlane(poisExceptJ1, 80);
            appendLine("分割平面 J2 的参数 A", J2.A.ToString("F6"));
            appendLine("分割平面 J2 的参数 B", J2.B.ToString("F6"));
            appendLine("分割平面 J2 的参数 C", J2.C.ToString("F6"));
            appendLine("分割平面 J2 的参数 D", J2.D.ToString("F6"));
            appendLine("分割平面 J2 的内部点数量", J2.InternalPoiCount);
            appendLine("分割平面 J2 的外部点数量", J2.ExternalPoiCount);
            // 3.点云水平截面投影
            var p5CastJ1 = J1.GetPoiProjectiveCoordinate(p5);
            appendLine("P5 点到最佳分割面（J1）的投影坐标 xt", p5CastJ1.X.ToString("F3"));
            appendLine("P5 点到最佳分割面（J1）的投影坐标 yt", p5CastJ1.Y.ToString("F3"));
            appendLine("P5 点到最佳分割面（J1）的投影坐标 zt", p5CastJ1.Z.ToString("F3"));
            var p800CastJ1 = J1.GetPoiProjectiveCoordinate(Points[799]);
            appendLine("P800 点到最佳分割面（J1）的投影坐标 xt", p800CastJ1.X.ToString("F3"));
            appendLine("P800 点到最佳分割面（J1）的投影坐标 yt", p800CastJ1.Y.ToString("F3"));
            appendLine("P800 点到最佳分割面（J1）的投影坐标 zt", p800CastJ1.Z.ToString("F3"));
            // 传出J1和J2平面用于：三、计算结果报告
            _j1 = J1;_j2 = J2;

            return sb.ToString();
        }

        private Plane? _j1, _j2;

        /// <summary>
        /// 三、计算结果报告
        /// </summary>
        /// <param name="result">点云分割结果</param>
        /// <returns>是否成功</returns>
        public bool GetPoiDivideResult(out string? result)
        {
            if(_j1 == null || _j2 == null)
            {
                result = null;
                return false;
            }
            StringBuilder sb = new();
            sb.AppendLine("点名, X, Y, Z, 标识");
            foreach(Point p in Points)
            {
                string label = "0";
                if (_j1.IsContainPoint(p)) label = "J1";
                else if (_j2.IsContainPoint(p)) label = "J2";
                sb.AppendLine($"{p.Name}, {p.X:F3}, {p.Y:F3}, {p.Z:F3}, {label}");
            }
            result = sb.ToString();
            return true;
        }

        /// <summary>
        /// 读取数据文件
        /// </summary>
        /// <param name="dataPath">数据文件路径</param>
        /// <returns>点云数据列表</returns>
        private static List<Point> LoadingData(string dataPath)
        {
            using StreamReader sr = new(dataPath);
            sr.ReadLine();
            List<Point> datas = [];
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] lineSplit = line.Split(',', StringSplitOptions.TrimEntries);
                datas.Add(new Point
                {
                    Name = lineSplit[0],
                    X = double.Parse(lineSplit[1]),
                    Y = double.Parse(lineSplit[2]),
                    Z = double.Parse(lineSplit[3]),
                });
            }
            return datas;
        }

        /// <summary>
        /// 利用海伦公式计算三点构成的面积
        /// </summary>
        /// <param name="p1">点1</param>
        /// <param name="p2">点2</param>
        /// <param name="p3">点3</param>
        /// <returns>三角形面积</returns>
        private static double GetTriangleArea(Point p1, Point p2, Point p3)
        {
            double a = p1.GetDistance(p2), b = p2.GetDistance(p3), c = p3.GetDistance(p1);
            double p = (a + b + c) / 2;
            return Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        /// <summary>
        /// 计算最佳分割平面
        /// </summary>
        /// <param name="points">点集</param>
        /// <param name="iteCount">迭代次数</param>
        /// <returns>平面</returns>
        private static Plane GetBestPlane(List<Point> points, int iteCount)
        {
            using List<Point>.Enumerator enu = points.GetEnumerator();
            Plane? J1 = null;
            for (int i = 0; i < iteCount; i++)
            {
                enu.MoveNext();
                Point p1 = enu.Current;
                enu.MoveNext();
                Point p2 = enu.Current;
                enu.MoveNext();
                Point p3 = enu.Current;
                Plane plane = new(p1, p2, p3, points);
                if (J1 == null || plane.InternalPoiCount > J1.InternalPoiCount) J1 = plane;
            }
            return J1!;
        }
    }
}
