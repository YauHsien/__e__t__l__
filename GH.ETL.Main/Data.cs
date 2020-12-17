using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class Data : IData
    {
        public string CustomerName { get; set; }
        public string IdNo { get; set; }
        public DateTime Birthday { get; set; }

        internal static int ComparisonForSign(Data x, Data y)
        {
            var c1 = String.Compare(x.IdNo, y.IdNo, true);
            if (c1 != 0)
                return c1;

            var c2 = DateTime.Compare(x.Birthday, y.Birthday);
            if (c2 != 0)
                return c2;

            return String.Compare(x.CustomerName, y.CustomerName, true);
        }
    }
}
