using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    internal class ChebychevI
    {
        public static readonly double Alpha = Math.Cos(5 * Math.PI / 8);
        public static readonly double Beta = Math.Cos(7 * Math.PI / 8);

        [IIRFilterAttr(FilterType.ChebychevTypeI, FilterPassType.BandPass)]
        public static IIRFilter BandPass(FilterParameters parameters)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.ChebychevTypeI, FilterPassType.BandStop)]
        public static IIRFilter BandStop(FilterParameters parameters)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.ChebychevTypeI, FilterPassType.HighPass)]
        public static IIRFilter HighPass(FilterParameters parameters)
        {
            throw new NotImplementedException();
        }

        [IIRFilterAttr(FilterType.ChebychevTypeI, FilterPassType.LowPass, 1, 2, 3, 4)]
        public static IIRFilter LowPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new Exception("Order not specified");

            if (parameters.RippleFactor == null)
                throw new Exception("Ripple factor not specified");

            if (parameters.Order < 1 || parameters.Order > 4)
                throw new Exception("Order must be beetween 1 and 4");

            int order = parameters.Order ?? 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double R = parameters.RippleFactor ?? 1;

            if (fs <= 0)
                throw new Exception("Sampling frequency must be positive.");

            if (fc < 0 || fc > fs / 2)
                throw new Exception("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / fs);
            double D, v_0, lambda, kappa;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 1:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, -0.5));
                    kappa = Math.Sinh(v_0);
                    D = kappa*gamma+1;
                    b[0] = kappa*gamma;
                    b[1] = b[0];
                    a[0] = kappa * gamma - 1;
                    break;
                case 2:
                    v_0 = 0.5 * Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, -0.5));
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(2, 2 * Math.Sqrt(2) * kappa, kappa * kappa + lambda * lambda).Evaluate(gamma).Real;
                        //(kappa*kappa+lambda*lambda) * gamma * gamma 
                        //+ 2 * Math.Sqrt(2) * kappa * gamma + 2;
                    b[0] = (kappa * kappa + lambda * lambda) * gamma * gamma;
                    b[1] = 2 * b[0];
                    b[2] = b[0];
                    a[0] = 2 * (kappa * kappa + lambda * lambda) * gamma * gamma - 4;
                    a[1] = (kappa * kappa + lambda * lambda) * gamma * gamma 
                        - 2 * Math.Sqrt(2) * kappa * gamma - 2;
                    break;
                case 3:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, -0.5)) / 3;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(4,8*kappa,5*kappa*kappa+3*lambda*lambda, kappa*(kappa*kappa + 3*lambda*lambda)).Evaluate(gamma).Real;
                    b[0] = kappa * (kappa*kappa+3*lambda*lambda) * Math.Pow(gamma, 3);
                    b[1] = 3 * b[0];
                    b[2] = 3 * b[0];
                    b[3] = b[0];
                    a[0] = new Polynomial(-12, -8 * kappa, 5*kappa*kappa+3*lambda*lambda, 3*kappa+(kappa*kappa+3*lambda*lambda)).Evaluate(gamma).Real;
                    a[1] = new Polynomial(12, -8 * kappa, -(5 * kappa * kappa + 3 * lambda * lambda),3 * kappa + (kappa * kappa + 3 * lambda * lambda)).Evaluate(gamma).Real; ;
                    a[2] = new Polynomial(-4, 8 * kappa, -(5 * kappa * kappa + 3 * lambda * lambda), 3 * kappa + (kappa * kappa + 3 * lambda * lambda)).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, -0.5)) / 4;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(1, 
                        -2*kappa*(Alpha+Beta), 
                        (1+Math.Sqrt(2))*kappa*kappa+lambda*lambda, 
                        -kappa*
                        (
                            Math.Sqrt(2)/2 *kappa*kappa*(Alpha+Beta)
                            +2 * lambda*lambda*(Math.Pow(Alpha,3)+Math.Pow(Beta,3))
                        ), 
                        (Math.Pow(kappa,4)+Math.Pow(lambda, 4))/8+3/4*kappa*kappa*lambda*lambda
                    ).Evaluate(gamma).Real;
                    b[0] = (Math.Pow(kappa, 4) + Math.Pow(lambda, 4)) / 8 + 3 / 4 * kappa * kappa * lambda * lambda;
                    b[1] = 4 * b[0];
                    b[2] = 6 * b[0];
                    b[3] = 4 * b[0];
                    b[4] = b[0];
                    a[0] = new Polynomial(
                        -4,
                        4 * kappa * (Alpha + Beta),
                        0,
                        -kappa *
                        (
                            Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),  
                        (Math.Pow(kappa, 4) + Math.Pow(lambda, 4)) / 2 + 3*kappa*kappa*lambda* lambda
                    ).Evaluate(gamma).Real;
                    a[1] = new Polynomial(
                        6,
                        0,
                        -2*((1+Math.Sqrt(2))*kappa*kappa+lambda*lambda),
                        0,
                        0.75*(Math.Pow(kappa, 4) + Math.Pow(lambda, 4)) + 4.5 * kappa * kappa * lambda * lambda
                    ).Evaluate(gamma).Real;
                    a[2] = new Polynomial(
                        -4,
                        4 * kappa * (Alpha + Beta),
                        0,
                        kappa *
                        (
                            Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),
                        (Math.Pow(kappa, 4) + Math.Pow(lambda, 4)) / 2 + 3 * kappa * kappa * lambda * lambda
                    ).Evaluate(gamma).Real;
                    a[3] = new Polynomial(
                        1,
                        2 * kappa * (Alpha + Beta),
                        (1 + Math.Sqrt(2)) * kappa * kappa + lambda * lambda,
                        kappa *
                        (
                            Math.Sqrt(2) / 2 * kappa * kappa * (Alpha + Beta)
                            + 2 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),
                        (Math.Pow(kappa, 4) + Math.Pow(lambda, 4)) / 8 + kappa * kappa * lambda * lambda * 3 / 4
                    ).Evaluate(gamma).Real;
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
