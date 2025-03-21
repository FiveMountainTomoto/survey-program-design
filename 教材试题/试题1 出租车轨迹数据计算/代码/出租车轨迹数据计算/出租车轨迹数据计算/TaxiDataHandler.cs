using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 出租车轨迹数据计算.DataStruct;
using static System.Math;

namespace 出租车轨迹数据计算
{
    internal class TaxiDataHandler
    {
        public List<TaxiData> TaxiDatas { get; }
        public List<Result>? Results { get; private set; }
        public TaxiDataHandler(string filePath)
        {
            TaxiDatas = [];
            using StreamReader sr = new(filePath);
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] lineSplit = line.Split(',');
                TaxiData taxi = new()
                {
                    ID = lineSplit[0],
                    TaxiStatus = (TaxiStatus)int.Parse(lineSplit[1]),
                    DateTime = DateTime.ParseExact(lineSplit[2].Trim(), "yyyyMMddHHmmss", null),
                    X = double.Parse(lineSplit[3]),
                    Y = double.Parse(lineSplit[4])
                };
                if (taxi.ID == "T2")
                    TaxiDatas.Add(taxi);
            }
        }

        /// <summary>
        /// 输出计算结果
        /// </summary>
        /// <returns>计算结果字符串</returns>
        public string OutputResult()
        {
            CalAllSpanResult();
            StringBuilder sb = new();
            sb.AppendLine("-------------速度和方位角计算结果-------------");
            foreach (Result r in Results!)
            {
                sb.AppendLine(OutputResultLine(r));
            }
            sb.AppendLine("-------------距离计算结果-------------");
            sb.AppendLine($"累积距离：{Results.Sum(r => r.Distance):F3}(km)");
            sb.AppendLine($"首尾直线距离：{GetDistance(TaxiDatas.First(), TaxiDatas.Last()):F3}(km)");
            return sb.ToString();
        }

        /// <summary>
        /// 时间转换为简化儒略日
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns>简化儒略日</returns>
        private static double GetDateTimeMJD(DateTime dateTime)
        {
            int Y = dateTime.Year, M = dateTime.Month, D = dateTime.Day, h = dateTime.Hour,
                N = dateTime.Minute, S = dateTime.Second;
            return -678987 + 367 * Y - (int)(7.0 / 4 * (Y + (int)((M + 9) / 12.0))) - (int)(275.0 * M / 9) + D + h / 24.0 + N / 1440.0 + S / 86400.0;
         // return -678987 + 367 * Y - (int)(7.0 / 4 * (Y + (int)((M + 9) / 12.0))) + (int)(275.0 * M / 9) + D + (h - 8) / 24.0 + N / 1440.0 + S / 86400.0;
         // 以上是作者源代码中写的公式，与书上给出的公式不一致，出租车数据也并没有标明是哪个时区。我的编码严格按照书本公式，结果和作者不同
        }

        /// <summary>
        /// 计算两个轨迹数据间的距离
        /// </summary>
        /// <param name="taxi1">第一个出租车轨迹数据</param>
        /// <param name="taxi2">第二个出租车轨迹数据</param>
        /// <returns>距离km</returns>
        private static double GetDistance(TaxiData taxi1, TaxiData taxi2)
        {
            return Sqrt(Pow(taxi2.X - taxi1.X, 2) + Pow(taxi2.Y - taxi1.Y, 2)) / 1000;
        }

        /// <summary>
        /// 计算两个轨迹数据间的速度并输出距离
        /// </summary>
        /// <param name="taxi1">第一个出租车轨迹数据</param>
        /// <param name="taxi2">第二个出租车轨迹数据</param>
        /// <param name="distance">距离</param>
        /// <returns>时段平均速度km</returns>
        private static double GetSpanSpeed(TaxiData taxi1, TaxiData taxi2, out double distance)
        {
            TimeSpan span = taxi2.DateTime - taxi1.DateTime;
            distance = GetDistance(taxi1, taxi2);
            return distance / span.TotalHours;
        }

        /// <summary>
        /// 计算两个轨迹数据间的方位角
        /// </summary>
        /// <param name="taxi1">第一个出租车轨迹数据</param>
        /// <param name="taxi2">第二个出租车轨迹数据</param>
        /// <returns>方位角 角度</returns>
        private static double GetSpanAzimuth(TaxiData taxi1, TaxiData taxi2)
        {
            double dy = taxi2.Y - taxi1.Y, dx = taxi2.X - taxi1.X;
            if (dx == 0)
            {
                if (dy > 0) return 90;
                else if (dy < 0) return 270;
                else return 0;
            }
            else
            {
                double azi = Atan2(dy , dx) / PI * 180;
                Func<double, double> standard = ang =>
                {
                    if (ang > 360) return ang - 360;
                    else if (ang < 0) return ang + 360;
                    else return ang;
                };
                if (dx < 0) return standard(azi + 180);
                else return standard(azi);
            }
        }

        /// <summary>
        /// 计算每个时段的结果
        /// </summary>
        private void CalAllSpanResult()
        {
            Results = [];
            for (int i = 1; i < TaxiDatas.Count; i++)
            {
                TaxiData first = TaxiDatas[i - 1], next = TaxiDatas[i];
                Result result = new()
                {
                    Number = i - 1,
                    BegMJD = GetDateTimeMJD(first.DateTime),
                    EndMJD = GetDateTimeMJD(next.DateTime),
                    Speed = GetSpanSpeed(first, next, out double dis),
                    Distance = dis,
                    Azimuth = GetSpanAzimuth(first, next)
                };
                Results.Add(result);
            }
        }

        /// <summary>
        /// 输出一个数据的计算结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns>一行速度和方位角计算结果</returns>
        private static string OutputResultLine(Result result)
        {
            return $"{result.Number:00}, {result.BegMJD:F5}-{result.EndMJD:F5}, {result.Speed:F3}, {result.Azimuth:F3}";
        }
    }
}
