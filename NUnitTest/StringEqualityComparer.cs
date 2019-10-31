using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnitTests
{
    public class StringEqualityComparer : IEqualityComparer<string>
    {

        public bool Equals(string x, string y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
