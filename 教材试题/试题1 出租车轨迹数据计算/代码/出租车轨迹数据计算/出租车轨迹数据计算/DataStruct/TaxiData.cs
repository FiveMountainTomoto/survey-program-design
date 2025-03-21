using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 出租车轨迹数据计算.DataStruct
{
    internal class TaxiData
    {
        public string? ID { get; set; }
        public TaxiStatus TaxiStatus { get; set; }
        public DateTime DateTime { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }
    internal enum TaxiStatus
    {
        Empty,
        Carrying,
        Standing,
        Cease,
        Else
    }
}
