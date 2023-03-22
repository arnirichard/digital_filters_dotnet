using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Butterworth
    {
        public static readonly double Alpha = -2 * (Math.Cos(5 * Math.PI / 8) + Math.Cos(7 * Math.PI / 8));
        public static readonly double Beta = 2 * (1 + 2 * Math.Cos(5 * Math.PI / 8) * Math.Cos(7 * Math.PI / 8));

        internal static IIRFilter BandStop(int order, int f_c, int f_s, double bw)
        {
            if (order <= 3)
                order = 2;
            else if (order != 4)
                order = 4;

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];
            switch (order)
            {
                case 2:
                    D = (1 + gamma * gamma) * f_c + gamma * bw;
                    b[0] = f_c * (gamma * gamma + 1);
                    b[1] = 2 * f_c * (gamma * gamma - 1);
                    b[2] = b[0];
                    a[0] = b[1];
                    a[1] = f_c * (gamma * gamma + 1) - gamma * bw;
                    break;
                case 4:
                default:
                    D = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) +
                        Math.Sqrt(2) * bw * f_c * gamma * (gamma * gamma + 1) +
                        bw * bw * gamma * gamma;
                    b[0] = f_c*f_c*(Math.Pow(gamma, 4)+2*gamma*gamma+1);
                    b[1] = 4 * f_c * f_c * (Math.Pow(gamma, 4) - 1);
                    b[2] = 4 * f_c * f_c * (3*Math.Pow(gamma, 4) - 2*gamma*gamma+3);
                    b[3] = b[1];
                    b[4] = b[0];
                    a[0] = 2 * (2 * f_c * f_c * (Math.Pow(gamma, 4) - 1)
                        + Math.Sqrt(2) * bw * f_c * gamma * (gamma * gamma - 1));
                    a[1] = 2 * (3 * f_c * f_c * (Math.Pow(gamma, 4) + 1)
                        - gamma * gamma * (2 * f_c * f_c + bw * bw));
                    a[2] = 2 * (2 * f_c * f_c * (Math.Pow(gamma, 4) - 1) +
                        Math.Sqrt(2) * bw * f_c * gamma * (1 - gamma * gamma));
                    a[3] = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) -
                        Math.Sqrt(2) * bw * f_c * gamma * (gamma * gamma + 1) +
                        bw * bw * gamma * gamma;
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

        public static IIRFilter BandPass(int order, int f_c, int f_s, double bw)
        {
            if (order <= 3)
                order = 2;
            else if (order != 4)
                order = 4;

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];
            switch(order)
            {
                case 2:
                    D = (1 + gamma * gamma) * f_c + gamma * bw;
                    b[0] = bw * gamma;
                    b[1] = 0;
                    b[2] = -b[0];
                    a[0] = 2 * f_c * (gamma * gamma - 1);
                    a[1] = f_c * (gamma * gamma + 1)-gamma*bw;
                    break;
                case 4:
                default:
                    D = f_c*f_c*(Math.Pow(gamma, 4)+2*gamma*gamma+1) +
                        Math.Sqrt(2)*bw*f_c*gamma*(gamma*gamma+1)+
                        bw*bw*gamma*gamma;
                    b[0] = bw * bw * gamma * gamma;
                    b[1] = 0;
                    b[2] = -2*b[0];
                    b[3] = 0;
                    b[4] = b[0];
                    a[0] = 2 * (2 * f_c * f_c * (Math.Pow(gamma, 4) - 1) 
                        + Math.Sqrt(2) * bw * f_c * gamma * (gamma * gamma - 1));
                    a[1] = 2 * (3 * f_c * f_c * (Math.Pow(gamma, 4) + 1) 
                        - gamma * gamma * (2 * f_c * f_c + bw * bw));
                    a[2] = 2 * (2 * f_c * f_c * (Math.Pow(gamma, 4) - 1) + 
                        Math.Sqrt(2)*bw*f_c*gamma*(1-gamma*gamma));
                    a[3] = f_c * f_c * (Math.Pow(gamma, 4) + 2 * gamma * gamma + 1) -
                        Math.Sqrt(2) * bw * f_c * gamma * (gamma * gamma + 1) +
                        bw * bw * gamma * gamma;
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

        public static IIRFilter HighPass(int order, int f_c, int f_s)
        {
            if (f_s <= 0)
                throw new Exception("Sampling frequency must be positive.");

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 1:
                    D = gamma + 1;
                    b[0] = 1;
                    b[1] = -1;
                    a[0] = gamma - 1;
                    break;
                case 2:
                    D = gamma * gamma + Math.Sqrt(2) * gamma + 1;
                    b[0] = 1;
                    b[1] = -2;
                    b[2] = 1;
                    a[0] = 2 * (gamma * gamma - 1);
                    a[1] = gamma * gamma - Math.Sqrt(2) * gamma + 1;
                    break;
                case 3:
                    D = Math.Pow(gamma, 3) + 2 * gamma * gamma + 2 * gamma + 1;
                    b[0] = 1;
                    b[1] = -3;
                    b[2] = 3;
                    b[3] = -1;
                    a[0] = new Polynomial(-3, -2, 2, 3).Evaluate(gamma).Real;
                    a[1] = new Polynomial(3, -2, -2, 3).Evaluate(gamma).Real;
                    a[2] = new Polynomial(-1, 2, -2, 1).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    D = new Polynomial(1, Alpha, Beta, Alpha, 1).Evaluate(gamma).Real;
                    b[0] = 1;
                    b[1] = -4;
                    b[2] = 6;
                    b[3] = -4;
                    b[4] = 1;
                    a[0] = 2 * new Polynomial(-2, -Alpha, 0, Alpha, 2).Evaluate(gamma).Real;
                    a[1] = 2 * new Polynomial(3, 0, -Beta, 0, 3).Evaluate(gamma).Real;
                    a[2] = 2 * new Polynomial(-2, Alpha, 0, -Alpha, 2).Evaluate(gamma).Real;
                    a[3] = new Polynomial(1, -Alpha, Beta, -Alpha, 1).Evaluate(gamma).Real;
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

        public static IIRFilter LowPass(int order, int f_c, int f_s)
        {
            if (f_s <= 0)
                throw new Exception("Sampling frequency must be positive.");

            if (f_c < 0 || f_c > f_s/2)
                throw new Exception("Cut-off must be positive and less than half F_s.");

            if (order < 1)
                order = 1;

            double gamma = Math.Tan(f_c * Math.PI / f_s);
            double D;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 1:
                    D = gamma + 1;
                    b[0] = gamma;
                    b[1] = gamma;
                    a[0] = gamma - 1;
                    break;
                case 2:
                    D = gamma * gamma + Math.Sqrt(2) * gamma + 1;
                    b[0] = gamma * gamma;
                    b[1] = 2 * b[0];
                    b[2] = b[0];
                    a[0] = 2 * (gamma * gamma - 1);
                    a[1] = gamma * gamma - Math.Sqrt(2) * gamma + 1;
                    break;
                case 3:
                    D = Math.Pow(gamma, 3) + 2 * gamma * gamma + 2 * gamma + 1;
                    b[0] = Math.Pow(gamma, 2);
                    b[1] = 2 * b[0];
                    b[2] = 2 * b[0];
                    b[3] = b[0];
                    a[0] = new Polynomial(-3, -2, 2, 3).Evaluate(gamma).Real;
                    a[1] = 3 * Math.Pow(gamma, 3) - 2 * Math.Pow(gamma, 2) - 2 * gamma + 3;
                    a[2] = new Polynomial(-1, 2, -2, 1).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    D = new Polynomial(1, Alpha, Beta, Alpha, 1).Evaluate(gamma).Real;
                    b[0] = Math.Pow(gamma, 4);
                    b[1] = 4 * b[0];
                    b[2] = 6 * b[0];
                    b[3] = 4 * b[0];
                    b[4] = b[0];
                    a[0] = 2 * new Polynomial(-2, -Alpha, 0, Alpha, 2).Evaluate(gamma).Real;
                    a[1] = 2 * new Polynomial(3, 0, -Beta, 0, 3).Evaluate(gamma).Real;
                    a[2] = 2 * new Polynomial(-2, Alpha, 0, -Alpha, 2).Evaluate(gamma).Real;
                    a[3] = new Polynomial(1, -Alpha, Beta, -Alpha, 1).Evaluate(gamma).Real;
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
