using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChromosomeExtraction.Extraction;
namespace ChromosomeExtraction
{
    class Program
    {
        static readonly double[,] transition ={
            /* a      c      g      t   */
            {0.359, 0.143, 0.167, 0.331},
            {0.384, 0.156, 0.023, 0.437},
            {0.305, 0.199, 0.150, 0.345 },
            {0.284, 0.182, 0.177, 0.357 }
            };
        const int a = 0;
        const int c = 1;
        const int g = 2;
        const int t = 3;


        static void Main(string[] args)
        {
            run();
        }
        static void getData()
        {
            int a = 100000;
            int b = 1099999;
            Extract(a, b, true);
            Debug.WriteLine("OK");
            Extract(1100000, 2099999);
        }
        static void run()
        {
            //double[,] transition =
            //{
            //    {0.7, 0.3},
            //    {0.4, 0.6}
            //}; 



            double[,] emission =
            {
                { 0.1, 0.4, 0.5 },
                { 0.6, 0.3, 0.1 },
                { 0.2, 0.8, 0.1 },
                { 0.2, 0.5, 0.3 }
            };

            double[] initial = { 0.2, 0.25, 0.45, 0.1 };

            HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);
            int[] sequence = new int[] { 0, 1, 2 };

            double logLikeliHood = hmm.Evaluate(sequence);

            // At this point, the log-likelihood of the sequence
            // occurring within the model is -3.3928721329161653.
            Console.WriteLine("logLikeliHood: {0}", logLikeliHood);
        }
    }
}
