using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    internal class VariableQ
    {
        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.BandPass, 4)]
        public static IIRFilter BandPass(FilterParameters parameters)
        {
            if (parameters.BW == null)
                throw new Exception("Bandwidth not specified");
            if (parameters.Q == null)
                throw new Exception("Q not specified");

            int order = 4;
            double Q = parameters.Q ?? 1;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double gamma = Math.Tan(fc * Math.PI / fs);
            double bw = parameters.BW ?? 100;

            double D = Q * fc * fc * Math.Pow(gamma, 4) 
                + bw * fc * (gamma * gamma + 1) * gamma 
                + Q * (2 * fc * fc + bw * bw) * gamma * gamma 
                + Q * fc * fc;

            double[] a = new double[order];
            double[] b = new double[order + 1];

            b[0] = Q*bw*bw*gamma*gamma;
            b[1] = 0;
            b[2] = -2*b[0];
            b[3] = 0;
            b[4] = b[0];
            a[0] = 2*fc*
                (
                2*Q*fc*Math.Pow(gamma,4)
                +bw*(gamma*gamma-1)*gamma
                -2*Q*fc
                );
            a[1] = 2*Q*
                (
                 3*fc*fc*Math.Pow(gamma, 4) 
                 - (2*fc*fc+bw*bw)*gamma*gamma
                 +3*fc*fc
                );
            a[2] = 2*fc*
                (
                 2*Q*fc*Math.Pow(gamma,4)
                 -bw*(gamma*gamma-1)*gamma
                 -2*Q*fc
                );
            a[3] = Q*fc*fc*Math.Pow(gamma,4)
                -bw*fc*(gamma*gamma+1)*gamma
                +Q*(2*fc*fc+bw*bw)*gamma*gamma
                +Q*fc*fc;

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

        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.BandStop, 4)]
        public static IIRFilter BandStop(FilterParameters parameters)
        {
            if (parameters.BW == null)
                throw new Exception("Bandwidth not specified");
            if (parameters.Q == null)
                throw new Exception("Q not specified");

            int order = 4;
            double Q = parameters.Q ?? 1;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double gamma = Math.Tan(fc * Math.PI / fs);
            double bw = parameters.BW ?? 100;

            double[] a = new double[order];
            double[] b = new double[order + 1];

            double D = Q * fc * fc * Math.Pow(gamma, 4) 
                - fc * bw * (gamma * gamma + 1) * gamma
                + Q * (2 * fc * fc + bw * bw) * gamma * gamma 
                + Q * fc * fc;

            b[0] = Q*fc*fc*(Math.Pow(gamma, 4)+2*gamma*gamma+1);
            b[1] = 4*Q*fc*fc*(Math.Pow(gamma, 4)-1);
            b[2] = 2* Q * fc * fc * (3*Math.Pow(gamma, 4) - 2 * gamma * gamma + 3);
            b[3] = b[1];
            b[4] = b[0];
            a[0] = 2*fc*(2*Q*fc*Math.Pow(gamma, 4)-bw*(gamma*gamma-1)*gamma-2*Q*fc);
            a[1] = 2 * Q * (3*fc*fc*Math.Pow(gamma, 4)-(2*fc*fc+bw*bw)*gamma*gamma+3*fc*fc);
            a[2] = 2*fc*(2*Q*fc*Math.Pow(gamma,4)+bw*(gamma*gamma-1)*gamma-2*Q*fc);
            a[3] = Q*fc*fc*Math.Pow(gamma, 4)+fc*bw*(gamma*gamma+1)*gamma+Q*(2*fc*fc+bw*bw)*gamma*gamma+Q*fc*fc;

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

        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.HighPass, 2)]
        public static IIRFilter HighPass(FilterParameters parameters)
        {
            if (parameters.Q == null)
                throw new Exception("Q not specified");

            int order = 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double Q = parameters.Q ?? 1;
            double gamma = Math.Tan(fc * Math.PI / fs);

            double D = Q * gamma * gamma + gamma + Q;

            double[] a = new double[order];
            double[] b = new double[order + 1];

            b[0] = Q;
            b[1] = -2*b[0];
            b[2] = b[0];
            a[0] = 2 * Q * (gamma * gamma - 1);
            a[1] = Q * gamma * gamma - gamma + Q;

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

        [IIRFilterAttr(FilterType.VariableQ, FilterPassType.LowPass, 2)]
        public static IIRFilter LowPass(FilterParameters parameters)
        {
            if (parameters.Q == null)
                throw new Exception("Q not specified");

            int order = 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;

            double Q = parameters.Q ?? 1;
            double gamma = Math.Tan(fc * Math.PI / fs);

            double D = Q * gamma * gamma + gamma + Q;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            b[0] = Q * gamma * gamma;
            b[1] = 2 * b[0];
            b[2] = b[0];
            a[0] = 2 * Q * (gamma * gamma - 1);
            a[1] = Q * gamma * gamma - gamma + Q;

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
