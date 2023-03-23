using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public class Bessel
    {
        [IIRFilterAttr(FilterType.Bessel, FilterPassType.BandPass)]
        public static IIRFilter BandPass(FilterParameters parameters)
        {
            return new IIRFilter(new double[0], new double[0], 1000, 10);
        }

        [IIRFilterAttr(FilterType.Bessel, FilterPassType.BandStop)]
        public static IIRFilter BandStop(FilterParameters parameters)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.Bessel, FilterPassType.HighPass)]
        public static IIRFilter HighPass(FilterParameters parameters)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.Bessel, FilterPassType.LowPass)]
        public static IIRFilter LowPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new Exception("Order not specified");

            if (parameters.Order < 1 || parameters.Order > 4)
                throw new Exception("Order must be beetween 1 and 4");

            int order = parameters.Order ?? 2;
            int f_c = parameters.Fc;
            int f_s = parameters.Fs;

            if (f_s <= 0)
                throw new Exception("Sampling frequency must be positive.");

            if (f_c < 0 || f_c > f_s / 2)
                throw new Exception("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 1:
                    return Butterworth.LowPass(parameters);
                case 2:
                    D = 3 * gamma * gamma + 3 * gamma + 1;
                    b[0] = 3 * gamma * gamma;
                    b[1] = 2 * b[0];
                    b[2] = b[0];
                    a[0] = 2 * (3*gamma * gamma - 1);
                    a[1] = 3 *gamma * gamma - 3 * gamma + 1;
                    break;
                case 3:
                    D = new Polynomial(1, 6, 15, 15).Evaluate(gamma).Real;
                    //15 * Math.Pow(gamma, 3) + 15 * gamma * gamma + 6 * gamma + 1;
                    b[0] = 15*Math.Pow(gamma, 3);
                    b[1] = 3 * b[0];
                    b[2] = 3 * b[0];
                    b[3] = b[0];
                    a[0] = 3* new Polynomial(1, -2, 5, 15).Evaluate(gamma).Real;
                    a[1] = 3 * new Polynomial(1, -2, 5, 15).Evaluate(gamma).Real;
                    a[2] = new Polynomial(-1, 6, -15, 15).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    D = new Polynomial(1, 10, 45, 105, 105).Evaluate(gamma).Real;
                    b[0] = 105*Math.Pow(gamma, 4);
                    b[1] = 4 * b[0];
                    b[2] = 6 * b[0];
                    b[3] = 4* b[0];
                    b[4] = b[0];
                    a[0] = 2 * new Polynomial(-2, -10, 0, 105, 210).Evaluate(gamma).Real;
                    a[1] = 2 * new Polynomial(1, 0, -15, 0, 105).Evaluate(gamma).Real;
                    a[2] = 2 * new Polynomial(-2, 10, 0, -105, 210).Evaluate(gamma).Real;
                    a[3] = new Polynomial(1, -10, 45, -105, 105).Evaluate(gamma).Real;
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

            return new IIRFilter(a, b, f_s, f_c);
        }
    }
}
