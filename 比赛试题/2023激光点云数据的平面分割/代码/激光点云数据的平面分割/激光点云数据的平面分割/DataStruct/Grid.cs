namespace 激光点云数据的平面分割.DataStruct
{
    public class Grid
    {
        private const double dx = 10, dy = 10;

        public Grid(int i, int j, IEnumerable<Point> points)
        {
            I = i;
            J = j;
            InternalPoints = points.Where(p => GetPoiGridRow(p) == i && GetPoiGridCol(p) == j).ToList();
            HeightAverage = InternalPoints.Average(p => p.Z);
            HeightMax = InternalPoints.Max(p => p.Z);
            HeightMin = InternalPoints.Min(p => p.Z);
            HeightDiff = HeightMax - HeightMin;
            HeightVar = InternalPoints.Sum(p => Math.Pow(p.Z - HeightAverage, 2)) / PoiCount;
        }

        public int I { get; set; }
        public int J { get; set; }

        public List<Point> InternalPoints { get; }

        public int PoiCount => InternalPoints.Count;

        public double HeightMax { get; }
        public double HeightMin { get; }
        public double HeightAverage { get; }
        public double HeightDiff { get; }
        public double HeightVar { get; }

        /// <summary>
        /// 计算点所在栅格的行
        /// </summary>
        /// <param name="p"></param>
        /// <returns>行数</returns>
        public static int GetPoiGridRow(Point p)
        {
            return (int)(p.Y / dy);
        }

        /// <summary>
        /// 计算点所在栅格的列
        /// </summary>
        /// <param name="p"></param>
        /// <returns>列数</returns>
        public static int GetPoiGridCol(Point p)
        {
            return (int)(p.X / dx);
        }
    }
}
