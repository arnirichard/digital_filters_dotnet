using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Extensions
    {
        public static double[] GetRange(this double start, double end, int length)
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
