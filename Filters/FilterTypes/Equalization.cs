using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Equalization
    {
        [IIRFilterAttr(FilterType.Equalization, FilterPassType.None, 2)]
        public static IIRFilter Create(FilterParameters parameters)
        {
            if (parameters.BW == null)
                throw new Exception("Bandwidth not specified");

            if (parameters.LinearGain == null)
                throw new Exception("LinearGain not specified");
            
            double bw = parameters.BW ?? 100;
            int fc = parameters.Fc;
            int fs = parameters.Fs;

            double alpha = Math.Tan(Math.PI * bw / fs);
            double beta = -Math.Cos(2*Math.PI * fc / fs);
            double g = (double)parameters.LinearGain;

            double D = g < 1 ? alpha + g : alpha + 1;
            double[] a = new double[2];
            double[] b = new double[3];

            if(g < 1)
            {
                b[0] = g * (1 + alpha);
                b[1] = 2*beta*g;
                b[2] = g*(1-alpha);
                a[0] = 2*beta*g;
                a[1] = g-alpha;
            }
            else
            {
                b[0] = 1 + g*alpha;
                b[1] = 2 * beta;
                b[2] = 1 - g * alpha;
                a[0] = 2*beta;
                a[1] = 1-alpha;
            }

            for (int i = 0; i < a.Length; i++)
            {
                a[i] /= D;
            }

            for (int i = 0; i < b.Length; i++)
            {
                b[i] /= D;
            }

            return new IIRFilter(a, b, parameters);
        }
    }
}
