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
        public Pharmacy pharmacy { get => _pharmacy; }
        private Pharmacy _pharmacy = new Pharmacy();
        public long timestamp { get; set; }
        public string hash { get; set; }

        public class Pharmacy
        {
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}
