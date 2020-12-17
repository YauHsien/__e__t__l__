using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class DataUpdateRequest
    {
        public ISet<Data> data { get; set; }
        public long timestamp { get; set; }
        public string hash { get; set; }
    }
}
