using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class DataComparer : IEqualityComparer<Data>
    {
        bool IEqualityComparer<Data>.Equals(Data x, Data y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return (x.Birthday.GetHashCode() ^ x.CustomerName.GetHashCode() ^ x.IdNo.GetHashCode()).GetHashCode()
                == (y.Birthday.GetHashCode() ^ y.CustomerName.GetHashCode() ^ y.IdNo.GetHashCode()).GetHashCode();
        }

        int IEqualityComparer<Data>.GetHashCode(Data obj)
        {
            return (obj.Birthday.GetHashCode() ^ obj.CustomerName.GetHashCode() ^ obj.IdNo.GetHashCode()).GetHashCode();
        }
    }
}
