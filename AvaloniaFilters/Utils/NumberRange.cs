using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters.Utils
{
    public class NumberRange<K> where K : INumber<K>
    {
        public K Start { get; }
        public K End { get; }
        public K Length => End - Start;

        public NumberRange(K start, K end)
        {
            Start = start;
            End = end;
        }

        public bool IsWithinRange(K val)
        {
            return val >= Start && val <= End;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Start.ToString(), End.ToString());
        }
    }
}
