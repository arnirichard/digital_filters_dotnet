using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public enum FilterType
    {
        Butterworth,
        LinkwitzReilly,
        Bessel,
        ChebychevTypeI,
        ChebychevTypeII,
        VariableQ,
        AllPass,
        Equalization,
        Notch,
        Shelf
    }

    public enum FilterPassType
    {
        None,
        LowPass,
        HighPass,
        BandPass,
        BandStop
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    class IIRFilterAttr : Attribute
    {
        public FilterType FilterType;
        public FilterPassType FilterPassType;
        public int[] Orders;

        public IIRFilterAttr(FilterType filterType,
            FilterPassType filterPassType = FilterPassType.None,
            params int[] orders)
        {
            Orders = orders;
            FilterType = filterType;
            FilterPassType = filterPassType;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is IIRFilterAttr fc &&
                fc.FilterType == FilterType &&
                fc.FilterPassType == FilterPassType;
        }

        public override int GetHashCode()
        {
            return 100 * (int)FilterType + (int)FilterPassType;
        }
    }
}
