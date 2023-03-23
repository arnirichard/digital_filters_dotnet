using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace Filters
{
    public class Polynomial
    {
        // a_0 + a_1*x + ... + a_n * x^n
        public readonly Complex[] Coefficients;
        public int Degree => Coefficients.Length-1;

        public Polynomial()
        {
            Coefficients = new Complex[] { 1 };
        }

        // a[0] + a[1]x + ... + a[n]x^n
        public Polynomial(params double[] a) : this(a.Select(x => new Complex(x, 0)).ToArray()) {}

        public Polynomial(params Complex[] a)
        {
            int toIndex = 0;
            for (int i = a.Length-1; i > -1; i--)
            {
                if (a[i] != 0)
                {
                    toIndex = i;
                    break;
                }
            }
            Coefficients = a.ToList().GetRange(0, toIndex+1).ToArray();
        }

        public Complex Evaluate(Complex x)
        {
            Complex result = new Complex();
            Complex power = 1;
            for(int i = 0; i < Coefficients.Length; i++)
            {
                result += Coefficients[i] * power;
                power *= x;
            }
            return result;
        }

        public static Polynomial FromRoots(params Complex[] roots)
        {
            if(roots.Length == 0)
                return new Polynomial();
            Polynomial result = new Polynomial();
            foreach (var r in roots)
                result *= new Polynomial(-r, 1);
            return result;
        }

        public static Polynomial operator *(Polynomial a, Polynomial b) 
        {
            Complex[] result = new Complex[a.Degree+ b.Degree + 1];

            for(int ai = 0; ai < a.Coefficients.Length; ai++)
            {
                for (int bi = 0; bi < a.Coefficients.Length; bi++)
                {
                    result[ai + bi] += a.Coefficients[ai] * b.Coefficients[bi];
                }
            }

            return new Polynomial(result);
        }

        public override string ToString()
        {
            List<string> chunks = new List<string>();

            for(int i = 0; i < Coefficients.Length; i++)
            {
                if (Coefficients[i] != 0)
                    chunks.Add(Coefficients[i].GetString()+ (i == 0 ? "" : ("x" + (i > 1 ? "^"+i : ""))));
            }

            return string.Join("+", chunks);
        }

        public Complex[] Roots()
        {
            switch (Degree)
            {
                case -1: // Zero-polynomial
                case 0: // Non-zero constant: y = a0
                    return Array.Empty<Complex>();
                case 1: // Linear: y = a0 + a1*x
                    return new[] { -Coefficients[0] / Coefficients[1] };
            }

            DenseMatrix? A = EigenvalueMatrix();
            if (A != null)
            {
                Evd<Complex> eigen = A.Evd(Symmetricity.Asymmetric);
                return eigen.EigenValues.AsArray();
            }

            throw new Exception("Roots not found");
        }

        /// <summary>
        /// Get the eigenvalue matrix A of this polynomial such that eig(A) = roots of this polynomial.
        /// </summary>
        /// <returns>Eigenvalue matrix A</returns>
        /// <note>This matrix is similar to the companion matrix of this polynomial, in such a way, that it's transpose is the columnflip of the companion matrix</note>
        public DenseMatrix? EigenvalueMatrix()
        {
            int n = Degree;
            if (n < 2)
            {
                return null;
            }

            // Negate, and normalize (scale such that the polynomial becomes monic)
            double aN = Coefficients[n].Real;
            double[] p = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                p[i] = -Coefficients[i].Real / aN;
            }

            DenseMatrix A0 = DenseMatrix.CreateDiagonal(n - 1, n - 1, 1.0);
            DenseMatrix A = new DenseMatrix(n);

            A.SetSubMatrix(1, 0, A0);
            A.SetRow(0, p.Reverse().Select(d => new Complex(d, 0)).ToArray());

            return A;
        }
    }
}
