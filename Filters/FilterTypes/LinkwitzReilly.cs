using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public class LinkwitzReilly
    {
        [IIRFilterAttr(FilterType.LinkwitzReilly, FilterPassType.BandPass, 4)]
        public static IIRFilter BandPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");
            if (parameters.Order != 4)
                throw new ArgumentException("Order must be 2 or 4");
            if(parameters.BW == null)
                throw new ArgumentException("BW not specified");

            int order = 4;
            int f_c = parameters.Fc;
            int f_s = parameters.Fs;
            double bw = parameters.BW ?? 100;

            if (f_s <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (f_c < 0 || f_c > f_s / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            D = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) +
                2 * bw * f_c * gamma * (gamma * gamma + 1) +
                bw * bw * gamma * gamma;
            b[0] = bw * bw * gamma * gamma;
            b[1] = 0;
            b[2] = -2 * b[0];
            b[3] = 0;
            b[4] = b[0];
            a[0] = 4 * (f_c * f_c * (Math.Pow(gamma, 4) - 1)
                + bw * f_c * gamma * (gamma * gamma - 1));
            a[1] = 2 * (3 * f_c * f_c * (Math.Pow(gamma, 4) + 1)
                - gamma * gamma * (2 * f_c * f_c + bw * bw));
            a[2] = 4 * (f_c * f_c * (Math.Pow(gamma, 4) - 1) +
                bw * f_c * gamma * (1 - gamma * gamma));
            a[3] = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) -
                2 * bw * f_c * gamma * (gamma * gamma + 1) +
                bw * bw * gamma * gamma;

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

        [IIRFilterAttr(FilterType.LinkwitzReilly, FilterPassType.BandStop, 4)]
        public static IIRFilter BandStop(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");
            if (parameters.Order != 4)
                throw new ArgumentException("Order must be 4");
            if (parameters.BW == null)
                throw new ArgumentException("BW not specified");

            int order = 4;
            int f_c = parameters.Fc;
            int f_s = parameters.Fs;
            double bw = parameters.BW ?? 100;

            if (f_s <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (f_c < 0 || f_c > f_s / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            D = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) +
                2 * bw * f_c * gamma * (gamma * gamma + 1) +
                bw * bw * gamma * gamma;
            b[0] = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1);
            b[1] = 4 * f_c * f_c * (Math.Pow(gamma, 4) - 1);
            b[2] = 2 * f_c * f_c * (3*Math.Pow(gamma, 4) - 2 * gamma * gamma + 3);
            b[3] = b[1];
            b[4] = b[0];
            a[0] = 4 * (f_c * f_c * (Math.Pow(gamma, 4) - 1)
                + bw * f_c * gamma * (gamma * gamma - 1));
            a[1] = 2 * (3 * f_c * f_c * (Math.Pow(gamma, 4) + 1)
                - gamma * gamma * (2 * f_c * f_c + bw * bw));
            a[2] = 4 * (f_c * f_c * (Math.Pow(gamma, 4) - 1) +
                bw * f_c * gamma * (1 - gamma * gamma));
            a[3] = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) -
                2 * bw * f_c * gamma * (gamma * gamma + 1) +
                bw * bw * gamma * gamma;

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

        [IIRFilterAttr(FilterType.LinkwitzReilly, FilterPassType.HighPass, 2, 4)]
        public static IIRFilter HighPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");
            if (parameters.Order != 2 && parameters.Order != 4)
                throw new ArgumentException("Order must be 2 or 4");

            int order = parameters.Order ?? 2;
            int f_c = parameters.Fc;
            int f_s = parameters.Fs;

            if (f_s <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (f_c < 0 || f_c > f_s / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 2:
                    D = gamma * gamma + 2 * gamma + 1;
                    b[0] = 1;
                    b[1] = -2;
                    b[2] = 1;
                    a[0] = 2 * (gamma * gamma - 1);
                    a[1] = gamma * gamma - 2 * gamma + 1;
                    break;
                case 4:
                default:
                    D = new Polynomial(1, 2 * Math.Sqrt(2), 4, 2 * Math.Sqrt(2), 1).Evaluate(gamma).Real;
                    b[0] = 1;
                    b[1] = -4;
                    b[2] = 6;
                    b[3] = -4;
                    b[4] = 1;
                    a[0] = 4 * new Polynomial(1, Math.Sqrt(2), 0, -Math.Sqrt(2), -1).Evaluate(gamma).Real;
                    a[1] = 2 * new Polynomial(3, 0, -4, 0, 3).Evaluate(gamma).Real;
                    a[2] = 4 * new Polynomial(1, -Math.Sqrt(2), 0, Math.Sqrt(2), -1).Evaluate(gamma).Real;
                    a[3] = new Polynomial(1, -2 * Math.Sqrt(2), 4, -2 * Math.Sqrt(2), 1).Evaluate(gamma).Real;
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

        [IIRFilterAttr(FilterType.LinkwitzReilly, FilterPassType.LowPass, 2, 4)]
        public static IIRFilter LowPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");
            if (parameters.Order != 2 && parameters.Order != 4)
                throw new ArgumentException("Order must be 2 or 4");
            
            int order = parameters.Order ?? 2;
            int f_c = parameters.Fc;
            int f_s = parameters.Fs;

            if (f_s <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (f_c < 0 || f_c > f_s / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 2:
                    D = gamma * gamma + 2 * gamma + 1;
                    b[0] = gamma * gamma;
                    b[1] = 2 * b[0];
                    b[2] = b[0];
                    a[0] = 2 * (gamma * gamma - 1);
                    a[1] = gamma * gamma - 2 * gamma + 1;
                    break;
                case 4:
                default:
                    D = new Polynomial(1, 2*Math.Sqrt(2), 4, 2 * Math.Sqrt(2), 1).Evaluate(gamma).Real;
                    b[0] = Math.Pow(gamma, 4);
                    b[1] = 4 * b[0];
                    b[2] = 6 * b[0];
                    b[3] = 4 * b[0];
                    b[4] = b[0];
                    a[0] = 4 * new Polynomial(1, Math.Sqrt(2), 0, -Math.Sqrt(2), -1).Evaluate(gamma).Real;
                    a[1] = 2 * new Polynomial(3, 0, -4, 0, 3).Evaluate(gamma).Real;
                    a[2] = 4 * new Polynomial(1, -Math.Sqrt(2), 0, Math.Sqrt(2), -1).Evaluate(gamma).Real;
                    a[3] = new Polynomial(1, -2* Math.Sqrt(2), 4, -2* Math.Sqrt(2), 1).Evaluate(gamma).Real;
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
