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
        [IIRFilterAttr(FilterType.Bessel, FilterPassType.BandPass, 4)]
        public static IIRFilter BandPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");
            if (parameters.Order != 4)
                throw new ArgumentException("Order must be 4");
            if (parameters.BW == null)
                throw new ArgumentException("BW not specified");

            int order = 4;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double bw = parameters.BW ?? 100;

            if (fs <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (fc < 0 || fc > fs / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / fs);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            D = new Polynomial(fc*fc, 
                3*bw*fc,
                2*fc*fc + 3 * bw*bw,
                3 * bw * fc,
                fc * fc).Evaluate(gamma).Real;

            b[0] = 3 * bw * bw * gamma * gamma;
            b[1] = 0;
            b[2] = -2*b[0];
            b[3] = 0;
            b[4] = b[0];
            a[0] = 2 * fc *
                (2 * fc * Math.Pow(gamma, 4)
                +3 * bw * (gamma * gamma * gamma - gamma)
                - 2 * fc
                );
            a[1] = 2 * 
                (
                3*fc*fc*Math.Pow(gamma, 4) 
                - (2*fc*fc+3*bw*bw) * gamma * gamma 
                + 3 * fc * fc
                );
            a[2] = 2*fc * 
                (
                2*fc*Math.Pow(gamma, 4) 
                - 3 * bw * (gamma*gamma*gamma-gamma) 
                - 2 * fc
                );
            a[3] = new Polynomial(fc*fc, 
                -3*bw*fc, 
                2*fc*fc+3*bw*bw, 
                -3*bw*fc, 
                fc*fc).Evaluate(gamma).Real;

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

        [IIRFilterAttr(FilterType.Bessel, FilterPassType.BandStop, 4)]
        public static IIRFilter BandStop(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");
            if (parameters.Order != 4)
                throw new ArgumentException("Order must be 4");
            if (parameters.BW == null)
                throw new ArgumentException("BW not specified");

            int order = 4;
            int fc = parameters.Fc;
            int f_s = parameters.Fs;
            double bw = parameters.BW ?? 100;

            if (f_s <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (fc < 0 || fc > f_s / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / f_s);
            double[] a = new double[order];
            double[] b = new double[order + 1];

            double D = new Polynomial(3* fc * fc,
                3 * bw * fc,
                6 * fc * fc + bw * bw,
                3 * bw * fc,
                3 * fc * fc).Evaluate(gamma).Real;

            b[0] = 3* fc * fc * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1);
            b[1] = 12 * fc * fc * (Math.Pow(gamma, 4) - 1);
            b[2] = 6 * fc * fc * (3 * Math.Pow(gamma, 4) - 2 * gamma * gamma + 3);
            b[3] = b[1];
            b[4] = b[0];
            a[0] = 6 * fc * 
                (
                    2 * fc * Math.Pow(gamma, 4)
                    + bw * (gamma * gamma * gamma - gamma)
                    - 2 * fc
                );
            a[1] = 2 * 
                (
                    9 * fc * fc * Math.Pow(gamma, 4)
                    -(6*fc*fc+bw*bw)*gamma*gamma 
                    +9*fc*fc
                );
            a[2] = 6 * fc * 
                (
                    2*fc* Math.Pow(gamma, 4)
                    - bw * (Math.Pow(gamma, 3)-gamma)
                    - 2 * fc
                );
            a[3] = new Polynomial(3*fc*fc, -3*bw*fc, 6*fc*fc+bw*bw,-3*bw*fc, 3 * fc * fc).Evaluate(gamma).Real;

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

        [IIRFilterAttr(FilterType.Bessel, FilterPassType.HighPass, 2, 3, 4)]
        public static IIRFilter HighPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");

            if (parameters.Order < 2 || parameters.Order > 4)
                throw new ArgumentException("Order must be beetween 2 and 4");

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
                    D = 3 * gamma * gamma + 3 * gamma + 3;
                    b[0] = 3;
                    b[1] = -6;
                    b[2] = 3;
                    a[0] = 2 * (gamma * gamma - 3);
                    a[1] = gamma * gamma - 3 * gamma + 3;
                    break;
                case 3:
                    D = new Polynomial(15, 15, 6, 1).Evaluate(gamma).Real;
                    //15 * Math.Pow(gamma, 3) + 15 * gamma * gamma + 6 * gamma + 1;
                    b[0] = 15;
                    b[1] = 45;
                    b[2] = -45;
                    b[3] = -15;
                    a[0] = 3 * new Polynomial(-15, -5, 2, 1).Evaluate(gamma).Real;
                    a[1] = 3 * new Polynomial(15, -5, -2, 1).Evaluate(gamma).Real;
                    a[2] = new Polynomial(-15, 15, -6, 1).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    D = new Polynomial(105, 105, 45, 10, 1).Evaluate(gamma).Real;
                    b[0] = 105;
                    b[1] = 4 * b[0];
                    b[2] = 6 * b[0];
                    b[3] = 4 * b[0];
                    b[4] = b[0];
                    a[0] = 2 * new Polynomial(-210, -105, 0, 10, 2).Evaluate(gamma).Real;
                    a[1] = 2 * new Polynomial(105, 0, -15, 0, 1).Evaluate(gamma).Real;
                    a[2] = 2 * new Polynomial(-210, 105, 0, -10, 2).Evaluate(gamma).Real;
                    a[3] = new Polynomial(105, -105, 45, -10, 1).Evaluate(gamma).Real;
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

        [IIRFilterAttr(FilterType.Bessel, FilterPassType.LowPass, 1, 2, 3, 4)]
        public static IIRFilter LowPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");

            if (parameters.Order < 1 || parameters.Order > 4)
                throw new ArgumentException("Order must be beetween 1 and 4");

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

            return new IIRFilter(a, b, parameters);
        }
    }
}
