using System;
using System.Data;
using System.Numerics;
using System.Text;

namespace Filters
{
    public enum FilterType
    {
        Butterworth,
        LinkwitzReilly,
        Bessel,
        ChebychevTypeI,
        ChebychevTypeII,
        VariableQ,
        AllPass,
        Equalization,
        Notch,
        Shelf
    }

    public class IIRFilter
    {
        // y_n = b_0 * x_n + b_1 * x_n-1 + ... + b_k * x_n-k + a_0 * y_n-1 + a_1 * y_n-2 + ... + a_m * y_n-m-1
        // H(z) = nominator/denominator
        // nominator = b_0 + b_1*z^-1 + ... + b_k*z^-k
        // denominator = 1 + a_0*z^-1 + ... + a_m * z^(-m-1)
        public readonly double[] A, B;
        public readonly Complex[] Poles, Zeros;
        public readonly Polynomial Nominator, Denominator;
        // Sampling rate, cut-off freq
        public readonly int Fs, Fc;

        public IIRFilter(double[] a, double[] b, int fs, int fc) 
        { 
            A = a;
            B = b;
            Fs = fs;
            Fc = fc;
            Nominator = new Polynomial(b.Reverse().ToArray());
            List<double> denomCoeffs = new List<double>() { 1 };
            denomCoeffs.AddRange(a);
            //denomCoeffs.Reverse();
            Denominator = new Polynomial(denomCoeffs.ToArray());
            Poles = Denominator.Roots();
            Zeros = Nominator.Roots();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("y_n = ");

            for(int i = 0; i < B.Length; i++)
            {
                sb.Append((B[0] < 0 ? "- " : (i > 0 ? " + " : "")) + string.Format("{0}x_n{1}",
                    Math.Abs(B[0]),
                    i == 0 ? "" : -i));
            }

            for (int i = 0; i < A.Length; i++)
            {
                sb.Append((A[0] < 0 ? " - " : " + ") +
                    string.Format("{0}y_n{1}",
                        Math.Abs(A[0]),
                        -(i+1)));
            }

            return sb.ToString();
        }

        public Complex[] GetResponse(double[] omega)
        {
            Complex[] result = new Complex[omega.Length];
            Complex c;

            for (int i = 0; i < result.Length; i++)
            {
                c = new Complex(Math.Cos(-omega[i]), Math.Sin(-omega[i]));
                result[i] = Nominator.Evaluate(c) / Denominator.Evaluate(c);
            }

            return result;
        }

        public IIRFilter(Complex[] poles, Complex[] zeros)
        {
            Poles = poles;
            Zeros = zeros;
            Nominator = Polynomial.FromRoots(zeros);
            Denominator = Polynomial.FromRoots(poles);
            B = Nominator.Coefficients.Select(c => c.Real).ToArray();
            A = Denominator.Coefficients.ToList()
                .GetRange(1, Denominator.Coefficients.Length)
                .Select(c => c.Real).ToArray();
        }

        public T[] Filter<T>(Span<T> input) where T : INumber<T>
        {
            T[] result = new T[input.Length];

            double[] x = new double[A.Length];
            double[] y = new double[B.Length];
            double val;

            for (int i = 0; i < input.Length; i++)
            {
                val = 0;

                x[i % x.Length] = double.CreateSaturating(input[i]); // / gain;

                for (int j = 0; j < A.Length; j++)
                    val += x[(j + i + 1) % A.Length] * A[i];

                for (int j = 0; j < B.Length - 1; j++)
                    val -= y[(j + i + 1) % A.Length] * A[i];

                y[i % y.Length] = val;

                result[i] = T.CreateChecked(val);
            }

            return result;
        }

        public static IIRFilter CreateLowPass(FilterType type, int order, int f_c, int f_s)
        {
            if (order < 1)
                order = 1;

            IIRFilter result;

            switch (type)
            {
                case FilterType.Butterworth:
                    result = Butterworth.LowPass(order, f_c, f_s);
                    break;
                case FilterType.ChebychevTypeI:
                    result = ChebychevI.LowPass(order, f_c, f_s);
                    break;
                case FilterType.ChebychevTypeII:
                    result = ChebychevII.LowPass(order, f_c, f_s);
                    break;
                case FilterType.LinkwitzReilly:
                    result = LinkwitzReilly.LowPass(order, f_c, f_s);
                    break;
                case FilterType.Bessel:
                    result = Bessel.LowPass(order, f_c, f_s);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public static IIRFilter CreateHighPass(FilterType type, int order, int f_c, int f_s, double bw)
        {
            if (order < 1)
                order = 1;

            IIRFilter result;

            switch (type)
            {
                case FilterType.Butterworth:
                    result = Butterworth.HighPass(order, f_c, f_s);
                    break;
                case FilterType.ChebychevTypeI:
                    result = ChebychevI.HighPass(order, f_c, f_s);
                    break;
                case FilterType.ChebychevTypeII:
                    result = ChebychevII.HighPass(order, f_c, f_s);
                    break;
                case FilterType.LinkwitzReilly:
                    result = LinkwitzReilly.HighPass(order, f_c, f_s);
                    break;
                case FilterType.Bessel:
                    result = Bessel.HighPass(order, f_c, f_s);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public static IIRFilter CreateBandPass(FilterType type, int order, int f_c, int f_s, double bw)
        {
            if (order < 1)
                order = 1;

            IIRFilter result;

            switch (type)
            {
                case FilterType.Butterworth:
                    result = Butterworth.BandPass(order, f_c, f_s, bw);
                    break;
                case FilterType.ChebychevTypeI:
                    result = ChebychevI.BandPass(order, f_c, f_s, bw);
                    break;
                case FilterType.ChebychevTypeII:
                    result = ChebychevII.BandPass(order, f_c, f_s, bw);
                    break;
                case FilterType.LinkwitzReilly:
                    result = LinkwitzReilly.BandPass(order, f_c, f_s, bw);
                    break;
                case FilterType.Bessel:
                    result = Bessel.BandPass(order, f_c, f_s, bw);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }

        public static IIRFilter CreateBandStop(FilterType type, int order, int f_c, int f_s, double bw)
        {
            if (order < 1)
                order = 1;

            IIRFilter result;

            switch (type)
            {
                case FilterType.Butterworth:
                    result = Butterworth.BandStop(order, f_c, f_s, bw);
                    break;
                case FilterType.ChebychevTypeI:
                    result = ChebychevI.BandStop(order, f_c, f_s, bw);
                    break;
                case FilterType.ChebychevTypeII:
                    result = ChebychevII.BandStop(order, f_c, f_s, bw);
                    break;
                case FilterType.LinkwitzReilly:
                    result = LinkwitzReilly.BandStop(order, f_c, f_s, bw);
                    break;
                case FilterType.Bessel:
                    result = Bessel.BandStop(order, f_c, f_s, bw);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return result;
        }
    }
}