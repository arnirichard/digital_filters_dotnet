using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Shelf
    {
        [IIRFilterAttr(FilterType.Shelf, FilterPassType.None, 1, 2)]
        public static IIRFilter Create(FilterParameters filterParameters)
        {
            throw new NotImplementedException();
        }
    }
}
