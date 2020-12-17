using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class DataCache : IData
    {
        private readonly Data _base_data;

        public DataCache(Data data)
        {
            _base_data = data;
        }

        public DateTime CacheTime
        {
            get => DateTime.Now;
        }
        string IData.CustomerName { get => _base_data.CustomerName; set => throw new NotImplementedException(); }
        string IData.IdNo { get => _base_data.IdNo; set => throw new NotImplementedException(); }
        DateTime IData.Birthday { get => _base_data.Birthday; set => throw new NotImplementedException(); }

        public static explicit operator DataCache(Data data)
        {
            var model = new DataCache(data);
            return model;
        }
    }
}
