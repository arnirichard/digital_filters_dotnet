using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Filters
{
    public class IIRFilter
    {
        // y_n = b_0 * x_n + b_1 * x_n-1 + ... + b_k * x_n-k - a_0 * y_n-1 - a_1 * y_n-2 - ... - a_m * y_n-m-1
        // H(z) = nominator/denominator
        // nominator = b_0 + b_1*z^-1 + ... + b_k*z^-k
        // denominator = 1 + a_0*z^-1 + ... + a_m * z^(-m-1)
        public double[] A { get; init; }
        public double[] B { get; init; }
        public FilterParameters Parameters { get; init; }

        public readonly Complex[] Poles, Zeros;
        public readonly Polynomial Nominator, Denominator;        

        static Dictionary<IIRFilterAttr, MethodInfo> filterCreators = new();

        public IIRFilter(double[] a, double[] b, FilterParameters filterParameters)
        {
            A = a;
            B = b;
            Parameters = filterParameters;
            Nominator = new Polynomial(b.Reverse().ToArray());
            List<double> denomCoeffs = new List<double>() { 1 };
            denomCoeffs.AddRange(a);
            denomCoeffs.Reverse();
            Denominator = new Polynomial(denomCoeffs.ToArray());
            Poles = Denominator.Roots();
            Zeros = Nominator.Roots();
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

        public IIRFilter(Complex[] zeros, Complex[] poles, FilterParameters parameters)
        {
            Parameters = parameters;
            Poles = poles;
            Zeros = zeros;
            Nominator = Polynomial.FromRoots(zeros);
            Denominator = Polynomial.FromRoots(poles);
            B = Nominator.Coefficients.Select(c => c.Real).ToArray();
            A = Denominator.Coefficients.ToList()
                .GetRange(1, Denominator.Coefficients.Length)
                .Select(c => c.Real).ToArray();
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

        public static List<FilterPassType> GetFilterPassTypes(FilterType filterType)
        {
            List<FilterPassType> result = new();

            foreach (var kvp in filterCreators)
            {
                if (kvp.Key.FilterType == filterType)
                {
                    result.Add(kvp.Key.FilterPassType);
                }
            }

            return result.Distinct().OrderBy(t => t.ToString()).ToList();
        }

        public static int[] GetFilterOrders(FilterType filterType, FilterPassType passType)
        {
            List<FilterPassType> result = new();

            foreach (var kvp in filterCreators)
            {
                if (kvp.Key.FilterType == filterType && kvp.Key.FilterPassType == passType)
                {
                    return kvp.Key.Orders;
                }
            }

            return new int[0];
        }

        public T[] Filter<T>(Span<T> input) where T : INumber<T>
        {
            T[] result = new T[input.Length];

            double[] x = new double[B.Length];
            double[] y = new double[A.Length];
            double val;

            for (int i = 0; i < input.Length; i++)
            {
                val = 0;

                x[i % x.Length] = double.CreateSaturating(input[i]);

                for (int j = 0; j < B.Length; j++)
                    val += x[(i-j+B.Length) % B.Length] * B[j];

                for (int j = 0; j < A.Length; j++)
                    val -= y[(i - j - 1+A.Length) % A.Length] * A[j];

                y[i % y.Length] = val;

                result[i] = T.CreateChecked(val);
            }

            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("y_n = ");

            for (int i = 0; i < B.Length; i++)
            {
                sb.Append((B[i] < 0 ? "- " : (i > 0 ? " + " : "")) + string.Format("{0}x_n{1}",
                    Math.Abs(B[i]),
                    i == 0 ? "" : -i));
            }

            for (int i = 0; i < A.Length; i++)
            {
                sb.Append((A[i] < 0 ? " - " : " + ") +
                    string.Format("{0}y_n{1}",
                        Math.Abs(A[i]),
                        -(i + 1)));
            }

            return sb.ToString();
        }
    }
}