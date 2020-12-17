using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    interface IData
    {
        public string CustomerName { get; set; }
        public string IdNo { get; set; }
        public DateTime Birthday { get; set; }

    }
}
