using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Reflection;
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

    public enum FilterPassType
    {
        None,
        LowPass,
        HighPass,
        BandPass,
        BandStop
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    class IIRFilterAttr: Attribute
    {
        public FilterType FilterType;
        public FilterPassType FilterPassType;

        public IIRFilterAttr(FilterType filterType, FilterPassType filterPassType = FilterPassType.None)
        {
            FilterType = filterType;
            FilterPassType = filterPassType;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is IIRFilterAttr fc &&
                fc.FilterType == FilterType &&
                fc.FilterPassType == FilterPassType;
        }

        public override int GetHashCode()
        {
            return 100*(int)FilterType + (int)FilterPassType;
        }
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
        public readonly double? BandWidth;

        static Dictionary<IIRFilterAttr, MethodInfo> filterCreators = new();

        public IIRFilter(double[] a, double[] b, int fs, int fc, double? bandWidth = null)
        {
            A = a;
            B = b;
            Fs = fs;
            Fc = fc;
            Nominator = new Polynomial(b.Reverse().ToArray());
            List<double> denomCoeffs = new List<double>() { 1 };
            denomCoeffs.AddRange(a);
            Denominator = new Polynomial(denomCoeffs.ToArray());
            Poles = Denominator.Roots();
            Zeros = Nominator.Roots();
            BandWidth = bandWidth;
        }

        static IIRFilter()
        {
            InitFilterDelegates();
        }

        static void InitFilterDelegates()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes()
              .Where(t => string.Equals(t.Namespace, "Filters", StringComparison.Ordinal))
              .ToArray();
            foreach(var type in types)
            {
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var methodInfo in methodInfos)
                {
                    IIRFilterAttr? attr = methodInfo.GetCustomAttributes(typeof(IIRFilterAttr), true).FirstOrDefault() as IIRFilterAttr;
                    if (attr != null)
                        filterCreators[attr] = methodInfo;
                }
            }
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

        public static IIRFilter? CreateFilter(FilterType type,
            FilterParameters filterParameters,
            FilterPassType passType = FilterPassType.None)
        {
            MethodInfo? methodInfo;
            IIRFilterAttr attr = new IIRFilterAttr(type, passType);
            if (filterCreators.TryGetValue(attr, out methodInfo))
            {
                try
                {
                    return methodInfo.Invoke(null, new object[] { filterParameters }) as IIRFilter;
                }
                catch{}
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("y_n = ");

            for (int i = 0; i < B.Length; i++)
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
                        -(i + 1)));
            }

            return sb.ToString();
        }
    }
}