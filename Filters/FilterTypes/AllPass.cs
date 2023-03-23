using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class AllPass
    {
        [IIRFilterAttr(FilterType.AllPass, FilterPassType.None, 1, 2)]
        public static IIRFilter Create(FilterParameters parameters)
        {
            if (parameters.Order < 1 || parameters.Order > 2)
            {
                throw new ArgumentException("Order must be between 1 and 2");
            }

            int order = parameters.Order ?? 2;

            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double bw = parameters.BW ?? 100;
            double gamma = Math.Tan(fc * Math.PI / fs);
            double[] a = new double[order];
            double[] b = new double[order + 1];
            double D;

            switch (order)
            {
                case 1:
                    D = gamma + 1;
                    b[0] = gamma - 1;
                    b[1] = D;
                    a[0] = b[0];
                    break;
                default:
                case 2:
                    double alpha = Math.Tan(Math.PI * bw / fs);
                    double beta = -Math.Cos(2 * Math.PI * fc / fs);
                    D = 1 + alpha;
                    b[0] = 1 - alpha;
                    b[1] = 2 * beta;
                    b[2] = D;
                    a[0] = b[1];
                    a[1] = b[0];
                    break;
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
