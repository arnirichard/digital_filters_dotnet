using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Extensions
    {
        public static double GetLogarithmicRatio(this double value, double start, double end, double logBase)
        {
            if(logBase == 1 || logBase <= 0)
                return GetLinearRatio(value, start, end);

            if (end == start)
            {
                return value == start ? 0 : (value < start ? -1 : 2);
            }

            double logRange = Math.Log(end/start, logBase);
            double valueRange = Math.Log(value/start, logBase);

            return logRange > 0
                ? valueRange / logRange
                : valueRange / -logRange;
        }

        // returns placement in range as ratio of range, with result = 0 if value == start, and result = 1 if value == end
        public static double GetLinearRatio(this double value, double start, double end)
        {
            double range = end - start;
            
            if(range == 0)
            {
                return value == start ? 0 : (value < start ? -1 : 2);
            }

            return range > 0
                ? (value - start) / range
                : (value - end) / -range;
        }

        public static double[] GetLinearRange(this double start, double end, int length)
        {
            double[] result = new double[length];

            if(length > 0) 
            { 
                double v = start;
                double delta = (end-start)/length;
                for(int i = 0; i < length; i++)
                {
                    result[i] = v;
                    v += delta;
                }
            }

            return result;
        }

        public static double[] GetLogarithmicRange(this double start, double end, int length, double logBase)
        {
            if (length == 0)
                return new double[0];

            if (logBase <= 1)
                return start.GetLinearRange(end, length);

            double[] result = new double[length];
            double range = end - start;

            if (range == 0)
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] = start;
            }
            else if(range > 0)
            {
                double logRange = Math.Log(end/start, logBase);
                double logDelta = logRange / length;
                double v = logDelta;
                result[0] = start;

                for (int i = 1; i < length; i++)
                {
                    v += logDelta;
                    result[i] = start * Math.Pow(logBase, v);
                }
            }
            else
            {
                double logRange = Math.Log(start / end, logBase);
                double logDelta = logRange / length;
                double v = logDelta;
                result[0] = start;

                for (int i = 1; i < length; i++)
                {
                    result[length-i] = end * Math.Pow(logBase, v);
                    v += logDelta;
                }
            }

            return result;
        }

        public static string GetString(this Complex c)
        {
            if (c == 0)
                return "0";

            if(c.Imaginary == 0)
                return c.Real.ToString();

            if (c.Real == 0)
                return c.Imaginary.ToString() + "j";

            return string.Format("{0}+{1}j",
                c.Real,
                c.Imaginary);
        }
    }
}
