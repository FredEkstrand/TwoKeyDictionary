using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests
{
    public class IntEqualityComparer : IEqualityComparer<int>
    {

        public bool Equals(int x, int y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(int obj)
        {
            return (obj.GetHashCode() ^ 89);
        }
    }
}
