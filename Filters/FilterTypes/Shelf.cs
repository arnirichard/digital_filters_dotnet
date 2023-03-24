using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public static class Shelf
    {
        [IIRFilterAttr(FilterType.Shelf, FilterPassType.Low, 1, 2)]
        public static IIRFilter CreateLow(FilterParameters parameters)
        {
            if (parameters.Order < 1 || parameters.Order > 2)
                throw new ArgumentException("Order must be between 1 or 2");

            if (parameters.LinearGain == null)
                throw new ArgumentException("LinearGain not specified");

            int order = parameters.Order ?? 1;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double g = (double)parameters.LinearGain;
            double gamma = Math.Tan(fc * Math.PI / fs);
            double[] a = new double[order];
            double[] b = new double[order+1];
            double D;

            switch (order)
            {
                case 1:
                    if (g > 1)
                    {
                        D = gamma + 1;
                        b[0] = g * gamma + 1;
                        b[1] = g * gamma - 1;
                        a[0] = gamma - 1;
                    }
                    else
                    {
                        D = gamma + g;
                        b[0] = g * (gamma + 1);
                        b[1] = g * (gamma - 1);
                        a[0] = gamma - g;
                    }
                    break;

                case 2:
                default:
                    double G = g > 2
                        ? g / Math.Sqrt(2)
                        : (g < 0.5 ? g * Math.Sqrt(2) : Math.Sqrt(g));
                    double g_d = Math.Pow((G * G - 1) / (g * g - G * G), 0.25);
                    double g_n = g_d * Math.Sqrt(g);
                    D = g_d * g_d * gamma * gamma + Math.Sqrt(2) * g_d * gamma + 1;
                    b[0] = g_n*g_n*gamma*gamma+Math.Sqrt(2)*g_n*gamma+1;
                    b[1] = 2*(g_n*g_n*gamma*gamma-1);
                    b[2] = g_n*g_n*gamma*gamma-Math.Sqrt(2)*g_n*gamma+1;
                    a[0] = 2*(g_d*g_d*gamma*gamma-1);
                    a[1] = g_d*g_d*gamma*gamma-Math.Sqrt(2)*g_d*gamma+1;
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

        [IIRFilterAttr(FilterType.Shelf, FilterPassType.High, 1, 2)]
        public static IIRFilter CreateHigh(FilterParameters parameters)
        {
            if (parameters.Order < 1 || parameters.Order > 2)
                throw new ArgumentException("Order must be between 1 or 2");

            if (parameters.LinearGain == null)
                throw new ArgumentException("LinearGain not specified");

            int order = parameters.Order ?? 1;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double g = (double)parameters.LinearGain;
            double gamma = Math.Tan(fc * Math.PI / fs);

            double[] a = new double[order];
            double[] b = new double[order + 1];
            double D;

            switch (order)
            {
                case 1:
                    if (g > 1)
                    {
                        D = gamma + 1;
                        b[0] = gamma + g;
                        b[1] = gamma - g;
                        a[0] = gamma - 1;
                    }
                    else
                    {
                        D = gamma *g + 1;
                        b[0] = g * (gamma + 1);
                        b[1] = g * (gamma - 1);
                        a[0] = gamma * g- 1;
                    }
                    break;

                case 2:
                default:
                    double G = g > 2
                        ? g / Math.Sqrt(2)
                        : (g < 0.5 ? g * Math.Sqrt(2) : Math.Sqrt(g));
                    double g_d = Math.Pow((G * G - 1) / (g * g - G * G), 0.25);
                    double g_n = g_d * Math.Sqrt(g);
                    D = gamma*gamma+Math.Sqrt(2)*g_d*gamma+g_d* g_d;
                    b[0] = gamma * gamma + Math.Sqrt(2) * g_n * gamma + g_n*g_n;
                    b[1] = 2*(gamma*gamma-g_n*g_n);
                    b[2] = gamma*gamma-Math.Sqrt(2)*g_n*gamma+g_n*g_n;
                    a[0] = 2*(gamma*gamma-g_d*g_d);
                    a[1] = gamma*gamma-Math.Sqrt(2)*g_d * gamma+g_d*g_d;
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
