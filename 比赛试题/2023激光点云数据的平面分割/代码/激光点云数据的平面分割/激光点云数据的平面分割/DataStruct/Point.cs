using static System.Math;
namespace 激光点云数据的平面分割.DataStruct
{
    public class Point
    {
        public string? Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        /// <summary>
        /// 计算两个点之间的距离
        /// </summary>
        /// <param name="another">另一点</param>
        /// <returns>两点距离</returns>
        public double GetDistance(Point another)
        {
            return Sqrt(Pow(X - another.X, 2) + Pow(Y - another.Y, 2) + Pow(Z - another.Z, 2));
        }
    }
}
