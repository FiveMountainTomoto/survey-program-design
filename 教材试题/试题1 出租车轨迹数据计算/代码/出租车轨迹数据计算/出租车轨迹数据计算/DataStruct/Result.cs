using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 出租车轨迹数据计算.DataStruct
{
    internal class Result
    {
        public int Number { get; set; }
        public double BegMJD { get; set; }
        public double EndMJD { get; set; }
        public double Speed { get; set; }
        public double Azimuth { get; set; }
        public double Distance { get; set; }
    }
}
