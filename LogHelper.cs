using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ChromosomeExtraction
{
    public static class LogHelper
    {
        /// <summary>
        ///   Computes log(1+x) without losing precision for small sample of x.
        /// </summary>
        /// 
        /// <remarks>
        ///   References:
        ///   - http://www.johndcook.com/csharp_log_one_plus_x.html
        /// </remarks>
        /// 
        public static double Log1p(double x)
        {
            if (x <= -1.0)
                return Double.NaN;

            if (System.Math.Abs(x) > 1e-4)
                return System.Math.Log(1.0 + x);

            // Use Taylor approx. log(1 + x) = x - x^2/2 with error roughly x^3/3
            // Since |x| < 10^-4, |x|^3 < 10^-12, relative error less than 10^-8
            return (-0.5 * x + 1.0) * x;
        }

        /// <summary>
        ///   Computes x + y without losing precision using ln(x) and ln(y).
        /// </summary>
        /// 
        public static double LogSum(double lna, double lnc)
        {
            if (lna == Double.NegativeInfinity)
                return lnc;
            if (lnc == Double.NegativeInfinity)
                return lna;

            if (lna > lnc)
                return lna + Log1p(Math.Exp(lnc - lna));

            return lnc + Log1p(Math.Exp(lna - lnc));
        }



        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        /// 
        public static double[,] Log(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            double[,] r = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = Math.Log(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise Exp operation.
        /// </summary>
        /// 
        public static double[,] Exp(this double[,] value)
        {
            int rows = value.GetLength(0);
            int cols = value.GetLength(1);

            double[,] r = new double[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    r[i, j] = Math.Exp(value[i, j]);
            return r;
        }

        /// <summary>
        ///   Elementwise Exp operation.
        /// </summary>
        /// 
        public static double[] Exp(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = Math.Exp(value[i]);
            return r;
        }


        /// <summary>
        ///   Elementwise Log operation.
        /// </summary>
        /// 
        public static double[] Log(this double[] value)
        {
            double[] r = new double[value.Length];
            for (int i = 0; i < value.Length; i++)
                r[i] = Math.Log(value[i]);
            return r;
        }

    }
}
