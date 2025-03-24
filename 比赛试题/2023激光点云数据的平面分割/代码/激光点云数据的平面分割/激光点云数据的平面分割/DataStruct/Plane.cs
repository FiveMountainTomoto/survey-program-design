using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Math;

namespace 激光点云数据的平面分割.DataStruct
{
    public class Plane
    {
        public const double eps = 0.1;
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public List<Point> InternalPoints { get; }
        public Point[] FittingPoints { get; }

        private int _totalPoiCount;
        public int InternalPoiCount => InternalPoints.Count;
        public int ExternalPoiCount => _totalPoiCount - InternalPoiCount - 3;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="p1">拟合点1</param>
        /// <param name="p2">拟合点2</param>
        /// <param name="p3">拟合点3</param>
        /// <param name="pois">点集</param>
        public Plane(Point p1, Point p2, Point p3, List<Point> pois)
        {
            FittingPoints = [p1, p2, p3];
            _totalPoiCount = pois.Count;
            A = (p2.Y - p1.Y) * (p3.Z - p1.Z) - (p3.Y - p1.Y) * (p2.Z - p1.Z);
            B = (p2.Z - p1.Z) * (p3.X - p1.X) - (p3.Z - p1.Z) * (p2.X - p1.X);
            C = (p2.X - p1.X) * (p3.Y - p1.Y) - (p3.X - p1.X) * (p2.Y - p1.Y);
            D = -A * p1.X - B * p1.Y - C * p1.Z;
            InternalPoints = pois.Except([p1, p2, p3]).Where(IsContainPoint).ToList();
        }

        /// <summary>
        /// 计算点到平面的距离
        /// </summary>
        /// <param name="p">点</param>
        /// <returns>距离</returns>
        public double GetDistance(Point p)
        {
            return Abs(A * p.X + B * p.Y + C * p.Z + D) / Sqrt(A * A + B * B + C * C);
        }

        /// <summary>
        /// 判断点是否在平面内部
        /// </summary>
        /// <param name="p">点</param>
        /// <returns>true或false</returns>
        public bool IsContainPoint(Point p)
        {
            return GetDistance(p) < eps;
        }

        public Point GetPoiProjectiveCoordinate(Point poi)
        {
            double x0 = poi.X, y0 = poi.Y, z0 = poi.Z;
            double tmp = A * A + B * B + C * C;
            double x = ((B * B + C * C) * x0 - A * (B * y0 + C * z0 + D)) / tmp;
            double y = ((A * A + C * C) * y0 - B * (A * x0 + C * z0 + D)) / tmp;
            double z = ((A * A + B * B) * z0 - C * (A * x0 + B * y0 + D)) / tmp;
            return new()
            {
                X = x,
                Y = y,
                Z = z
            };
        }
    }
}
