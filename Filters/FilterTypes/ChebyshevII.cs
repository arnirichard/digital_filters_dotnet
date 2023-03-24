﻿using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filters
{
    public class ChebyshevII
    {
        public static readonly double Alpha = Math.Cos(5 * Math.PI / 8);
        public static readonly double Beta = Math.Cos(7 * Math.PI / 8);

        [IIRFilterAttr(FilterType.ChebyshevTypeII, FilterPassType.BandStop, 2, 4)]
        public static IIRFilter BandStop(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");

            if (parameters.RippleFactor == null)
                throw new ArgumentException("Ripple factor not specified");

            if (parameters.BW == null)
                throw new ArgumentException("Bandwidth not specified");

            if (parameters.Order != 2 && parameters.Order != 4)
                throw new ArgumentException("Order must be 2 or 4");

            int order = parameters.Order ?? 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double R = parameters.RippleFactor ?? 1;
            double bw = parameters.BW ?? 100;

            if (fs <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (fc < 0 || fc > fs / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / fs);
            double D, v_0, lambda, kappa;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 2:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, 0.5));
                    kappa = Math.Sinh(v_0);
                    D = new Polynomial(fc,
                        bw*kappa,
                        fc).Evaluate(gamma).Real;
                    b[0] = fc * (gamma * gamma + 1);
                    b[1] = 2 * fc * (gamma * gamma - 1);
                    b[2] = b[0];
                    a[0] = b[1];
                    a[1] = fc*gamma*gamma-bw*kappa*gamma+fc;
                    break;
                case 4:
                default:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, 0.5)) / 2;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(
                        2*fc*fc,
                        2*Math.Sqrt(2)*kappa*fc*bw,
                        4*fc*fc+bw*bw*(kappa*kappa+lambda*lambda),
                        2 * Math.Sqrt(2) * kappa * fc* bw,
                        2*fc*fc
                    ).Evaluate(gamma).Real;

                    b[0] = 2*fc*fc*Math.Pow(gamma, 4)+(4*fc*fc+bw*bw)*gamma*gamma+2*fc*fc;
                    b[1] = 8*fc*fc*(Math.Pow(gamma, 4)-1);
                    b[2] = 2*(6*fc*fc*Math.Pow(gamma,4)-(4*fc*fc+bw*bw)*gamma*gamma+6*fc*fc);
                    b[3] = b[1];
                    b[4] = b[0];
                    a[0] = 4 * fc*(2*fc*Math.Pow(gamma,4)+Math.Sqrt(2)*kappa*bw*(gamma*gamma-1)*gamma-2*fc);
                    a[1] = 2*(6*fc*fc*Math.Pow(gamma, 4)-(4*fc*fc+bw*bw*(kappa*kappa+lambda*lambda))*gamma*gamma+6*fc*fc);
                    a[2] = 4*fc*(2*fc*Math.Pow(gamma, 4)-Math.Sqrt(2)*kappa*bw*(gamma*gamma-1)*gamma-2*fc);
                    a[3] = 2*fc*fc*Math.Pow(gamma, 4)
                        -2*Math.Sqrt(2)*kappa*fc*bw*(gamma*gamma+1)*gamma+
                        (4*fc*fc+bw*bw*(kappa*kappa+lambda*lambda))*gamma*gamma
                        +2*fc*fc;
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

        [IIRFilterAttr(FilterType.ChebyshevTypeII, FilterPassType.BandPass, 2, 4)]
        public static IIRFilter BandPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");

            if (parameters.RippleFactor == null)
                throw new ArgumentException("Ripple factor not specified");

            if (parameters.BW == null)
                throw new ArgumentException("Bandwidth not specified");

            if (parameters.Order != 2 && parameters.Order != 4)
                throw new ArgumentException("Order must be 2 or 4");

            int order = parameters.Order ?? 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double R = parameters.RippleFactor ?? 1;
            double bw = parameters.BW ?? 100;

            if (fs <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (fc < 0 || fc > fs / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / fs);
            double D, v_0, lambda, kappa;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 2:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, 0.5));
                    kappa = Math.Sinh(v_0);
                    D = kappa*fc*(gamma*gamma+1)+bw*gamma;
                    b[0] = bw*gamma;
                    b[1] = 0;
                    b[2] = -b[0];
                    a[0] = 2*kappa*fc * (gamma * gamma - 1);
                    a[1] = kappa * fc * (gamma*gamma+1)-bw*gamma;
                    break;
                case 4:
                default:
                    v_0 = Math.Asinh(Math.Pow(Math.Pow(10, R / 10) - 1, 0.5)) / 2;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(
                        fc*fc*(kappa*kappa+lambda*lambda),
                        2*Math.Sqrt(2)*kappa*fc*bw,
                        2*(fc*fc*(kappa*kappa+lambda*lambda)+bw*bw),
                        2 * Math.Sqrt(2) * kappa * fc * bw,
                        fc * fc * (kappa * kappa + lambda * lambda)
                    ).Evaluate(gamma).Real;
                    b[0] = fc*fc*Math.Pow(gamma,4)+2*(fc*fc+bw*bw)*gamma*gamma+fc*fc;
                    b[1] = 4*fc*fc*(Math.Pow(gamma, 4)-1);
                    b[2] = 2*(3*fc*fc*Math.Pow(gamma,4)-2*(fc*fc+bw*bw)*gamma*gamma+3*fc*fc);
                    b[3] = b[1];
                    b[4] = b[0];
                    a[0] = 4 * fc * new Polynomial(
                        -fc*(kappa*kappa+lambda*lambda),
                        -Math.Sqrt(2)*kappa*bw,
                        0,
                        Math.Sqrt(2) * kappa * bw,
                        fc*(kappa*kappa+lambda*lambda)
                    ).Evaluate(gamma).Real;
                    a[1] = 2* new Polynomial(
                        3*fc*fc*(kappa*kappa+lambda*lambda),
                        0,
                        -2*(fc*fc*(kappa*kappa+lambda*lambda)+bw*bw),
                        0,
                        3*fc*fc*(kappa*kappa+lambda*lambda)
                    ).Evaluate(gamma).Real;
                    a[2] = 3*fc* new Polynomial(
                        -fc*(kappa*kappa+lambda*lambda),
                        Math.Sqrt(2)*kappa*bw,
                        0,
                        -Math.Sqrt(2) * kappa * bw,
                        fc* (kappa * kappa + lambda * lambda)
                    ).Evaluate(gamma).Real;
                    a[3] = new Polynomial(
                        fc*fc * (kappa * kappa + lambda * lambda), 
                        -2*Math.Sqrt(2)*kappa*fc*bw,
                        2*(fc*fc*(kappa*kappa+lambda*lambda)+bw*bw),
                        -2 * Math.Sqrt(2) * kappa * fc * bw,
                        fc * fc * (kappa * kappa + lambda * lambda)
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

        [IIRFilterAttr(FilterType.ChebyshevTypeII, FilterPassType.HighPass, 1, 2, 3, 4)]
        public static IIRFilter HighPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");

            if (parameters.RippleFactor == null)
                throw new ArgumentException("Ripple factor not specified");

            if (parameters.Order < 1 || parameters.Order > 4)
                throw new ArgumentException("Order must be beetween 1 and 4");

            int order = parameters.Order ?? 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double R = parameters.RippleFactor ?? 1;

            if (fs <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (fc < 0 || fc > fs / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / fs);
            double D, v_0, lambda, kappa;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 1:
                    v_0 = Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1));
                    kappa = Math.Sinh(v_0);
                    D = kappa * gamma+1;
                    b[0] = 1;
                    b[1] = -1;
                    a[0] = gamma * kappa -1;
                    break;
                case 2:
                    v_0 = 0.5 * Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1));
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(2,
                        2 * Math.Sqrt(2) * kappa,
                        kappa * kappa + lambda * lambda).Evaluate(gamma).Real;
                    b[0] = gamma * gamma + 2;
                    b[1] = 2 * (gamma * gamma - 2);
                    b[2] = b[0];
                    a[0] = 2 * ((kappa * kappa + lambda * lambda)*gamma*gamma-2);
                    a[1] = (kappa * kappa + lambda * lambda) * gamma * gamma - 2 * Math.Sqrt(2)*kappa*gamma+2;
                    break;
                case 3:
                    v_0 = Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1)) / 3;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(
                        4,
                        8 * kappa,
                        5 * kappa * kappa + 3 * lambda * lambda,
                        kappa * (kappa * kappa + 3 * lambda * lambda)
                    ).Evaluate(gamma).Real;
                    b[0] = 3 * gamma * gamma + 4;
                    b[1] = 3 * (gamma * gamma - 4);
                    b[2] = -b[1];
                    b[3] = -b[0];
                    a[0] = new Polynomial(
                        -12,
                        -8 * kappa,
                        5 * kappa * kappa + 3 * lambda * lambda,
                        3 * kappa * (kappa * kappa + 3 * lambda * lambda)
                        ).Evaluate(gamma).Real;
                    a[1] = new Polynomial(
                        12,
                        -8 * kappa,
                        -(5 * kappa * kappa + 3 * lambda * lambda),
                        3 * kappa * (kappa * kappa + 3 * lambda * lambda)
                        ).Evaluate(gamma).Real;
                    a[2] = new Polynomial(
                        -4,
                        8 * kappa,
                        -(5 * kappa * kappa + 3 * lambda * lambda),
                        kappa * (kappa * kappa + 3 * lambda * lambda)
                        ).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    v_0 = Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1)) / 4;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(
                        8,
                        -16 * kappa * (Alpha + Beta),
                        8 * ((1 + Math.Sqrt(2)) * kappa * kappa + lambda * lambda),
                        -4 * kappa * (
                            Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),
                        Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * kappa * kappa * lambda * lambda
                    ).Evaluate(gamma).Real;
                    b[0] = Math.Pow(gamma, 4) + 8 * gamma * gamma + 8;
                    b[1] = 4 * (Math.Pow(gamma, 4) - 8);
                    b[2] = 2 * (3 * Math.Pow(gamma, 4) - 8 * gamma * gamma + 24);
                    b[3] = b[1];
                    b[4] = b[0];
                    a[0] = new Polynomial(
                        -32,
                       32*kappa*(Alpha+Beta),
                       0,
                       -8 * kappa * (
                            Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),
                       4*(Math.Pow(kappa,4)+Math.Pow(lambda,4)+6*Math.Pow(kappa,2) * lambda * lambda)
                    ).Evaluate(gamma).Real;
                    a[1] = new Polynomial(
                        48,
                        0,
                        -16*((1+Math.Sqrt(2))*kappa*kappa+lambda*lambda),
                        0,
                        6 * (Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * Math.Pow(kappa, 2) * lambda * lambda)
                    ).Evaluate(gamma).Real;
                    a[2] = new Polynomial(
                        -32,
                        -32 * kappa * (Alpha + Beta),
                       0,
                       8 * kappa * (
                            Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),
                       4 * (Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * Math.Pow(kappa, 2) * lambda * lambda)
                    ).Evaluate(gamma).Real;
                    a[3] = new Polynomial(
                        8,
                        16*kappa*(Alpha+Beta),
                        8 * ((1 + Math.Sqrt(2)) * kappa * kappa + lambda * lambda),
                        4 * kappa * (
                            Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                        ),
                        Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * Math.Pow(kappa, 2) * lambda * lambda
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

        [IIRFilterAttr(FilterType.ChebyshevTypeII, FilterPassType.LowPass, 1, 2, 3, 4)]
        public static IIRFilter LowPass(FilterParameters parameters)
        {
            if (parameters.Order == null)
                throw new ArgumentException("Order not specified");

            if (parameters.RippleFactor == null)
                throw new ArgumentException("Ripple factor not specified");

            if (parameters.Order < 1 || parameters.Order > 4)
                throw new ArgumentException("Order must be beetween 1 and 4");

            int order = parameters.Order ?? 2;
            int fc = parameters.Fc;
            int fs = parameters.Fs;
            double R = parameters.RippleFactor ?? 1;

            if (fs <= 0)
                throw new ArgumentException("Sampling frequency must be positive.");

            if (fc < 0 || fc > fs / 2)
                throw new ArgumentException("Cut-off must be positive and less than half F_s.");

            double gamma = Math.Tan(fc * Math.PI / fs);
            double D, v_0, lambda, kappa;
            double[] a = new double[order];
            double[] b = new double[order + 1];

            switch (order)
            {
                case 1:
                    v_0 = Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1));
                    kappa = Math.Sinh(v_0);
                    D = kappa + gamma;
                    b[0] = gamma;
                    b[1] = b[0];
                    a[0] = gamma - kappa;
                    break;
                case 2:
                    v_0 = 0.5 * Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1));
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(kappa * kappa + lambda * lambda,  2 * Math.Sqrt(2) * kappa, 2).Evaluate(gamma).Real;
                    b[0] = 2*gamma*gamma+1;
                    b[1] = 2*(2 * gamma * gamma - 1);
                    b[2] = b[0];
                    a[0] = 4*gamma * gamma - 2 * (kappa * kappa + lambda * lambda);
                    a[1] = 2*gamma*gamma-2*Math.Sqrt(2)*kappa*gamma+kappa*kappa+lambda*lambda;
                    break;
                case 3:
                    v_0 = Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1)) / 3;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(kappa * (kappa * kappa + 3 * lambda * lambda),
                        5 * kappa * kappa + 3 * lambda * lambda,
                        8 * kappa, 4).Evaluate(gamma).Real;
                    b[0] = gamma*(4*gamma*gamma+3);
                    b[1] = 3 * gamma *(4*gamma*gamma-1);
                    b[2] = b[1];
                    b[3] = b[0];
                    a[0] = new Polynomial(
                        -3 * kappa * (kappa * kappa + 3 * lambda * lambda),      
                        -(5 * kappa * kappa + 3 * lambda * lambda),
                        8 * kappa, 
                        12).Evaluate(gamma).Real;
                    a[1] = new Polynomial(
                        3 * kappa * (kappa * kappa + 3 * lambda * lambda),
                        -(5 * kappa *kappa + 3 * lambda * lambda) ,
                        - 8 * kappa, 
                        12).Evaluate(gamma).Real;
                    a[2] = new Polynomial(
                        -kappa*(kappa * kappa + 3 * lambda * lambda),
                        5 * kappa * kappa + 3 * lambda * lambda , 
                        - 8 * kappa, 
                        4).Evaluate(gamma).Real;
                    break;
                case 4:
                default:
                    v_0 = Math.Asinh(Math.Sqrt(Math.Pow(10, R / 10) - 1)) / 4;
                    kappa = Math.Sinh(v_0);
                    lambda = Math.Cosh(v_0);
                    D = new Polynomial(
                        Math.Pow(kappa,4)+Math.Pow(lambda,4)+ 6*kappa*kappa*lambda*lambda,
                        -4*kappa*(
                            Math.Sqrt(2)*kappa*kappa*(Alpha+Beta)
                            + 4 * lambda * lambda * (Math.Pow(Alpha, 3) +Math.Pow(Beta,3))
                        ),
                        8*((1+Math.Sqrt(2))*kappa*kappa+lambda*lambda),
                        -16*kappa*(Alpha+Beta),
                        8
                    ).Evaluate(gamma).Real;
                    b[0] = 8 * Math.Pow(gamma, 4) + 8 * gamma * gamma + 1; ;
                    b[1] = 4 * (8*Math.Pow(gamma, 4)-1);
                    b[2] = 2 * (24*Math.Pow(gamma, 4)-8*gamma*gamma+3);
                    b[3] = b[1];
                    b[4] = b[0];
                    a[0] = new Polynomial(
                        -4*(Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6*kappa*kappa*lambda*lambda),
                        8*kappa*(
                                Math.Sqrt(2)*kappa*kappa*(Alpha+Beta)
                                +4*lambda*lambda*(Math.Pow(Alpha,3)+Math.Pow(Beta,3))
                            ),
                        0,
                        -32*kappa*(Alpha+Beta),
                        32
                    ).Evaluate(gamma).Real;
                    a[1] = new Polynomial(
                        6 * (Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * kappa * kappa * lambda * lambda),
                        0,
                        -16*((1+Math.Sqrt(2))*kappa*kappa+lambda*lambda),
                        0,
                        48
                    ).Evaluate(gamma).Real;
                    a[2] = new Polynomial(
                        -4 * (Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * kappa * kappa * lambda * lambda),
                        -8 * kappa * (
                                Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                                + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                            ),
                        0,
                        32 * kappa * (Alpha + Beta),
                        32
                    ).Evaluate(gamma).Real;
                    a[3] = new Polynomial(
                        Math.Pow(kappa, 4) + Math.Pow(lambda, 4) + 6 * kappa * kappa * lambda * lambda,
                        4 * kappa * (
                                Math.Sqrt(2) * kappa * kappa * (Alpha + Beta)
                                + 4 * lambda * lambda * (Math.Pow(Alpha, 3) + Math.Pow(Beta, 3))
                            ),
                        8 * ((1 + Math.Sqrt(2)) * kappa * kappa + lambda * lambda),
                        16*kappa*(Alpha+Beta),
                        8
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
