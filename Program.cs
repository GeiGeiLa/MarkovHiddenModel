//#define LOGFLOAT
#define HIDDENPART
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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


        static void Main(string[] args)
        {
            Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var sw = new Stopwatch();
#if true
            double probability = 0.0f;
            sw.Start();
            foreach(var d in getData())
            {
                run(d, ref probability);
            }
            sw.Stop();
            Console.WriteLine("Prob:" + probability);
            Console.WriteLine("Time Consumption:" + sw.ElapsedMilliseconds);
            sw.Reset();
#endif
            sw.Start();
            double chainProb = chain();
            sw.Stop();
            Console.WriteLine("chain prob" +chainProb);
            Console.WriteLine("chain time consumption:" + sw.ElapsedMilliseconds);
            long totalBytesOfMemoryUsed = currentProcess.WorkingSet64;
            Console.WriteLine("Memory used:" + totalBytesOfMemoryUsed+ "bytes");
            Console.ReadKey(true);
        }
        static IEnumerable<int[]> getData()
        {
            const int windowSize = 19;
            int a = 100000;
            int b = 1099999;
            Extract(a, b, true);
            //Extract(1100000, 2099999);
            using StreamReader reader = new(@"S." + a + "_" + b + ".txt");
            string line = reader.ReadLine()!;
            int[] seq = new int[windowSize];
            for(int charIndex = 0; charIndex < line.Length; charIndex += windowSize)
            {
                for (int i = 0; i < windowSize; i++)
                {
                    seq[i] = line.ToCharArray()[charIndex + i] - '0';

                }
                yield return seq;
            }
        }
        static void run(int[] sequence, ref double probability)
        {
            double[,] emission =
            {
            /*    a     c    g    t   */
                { 0.1, 0.4, 0.2, 0.3 },
                { 0.5, 0.3, 0.1, 0.1 },
                { 0.2, 0.6, 0.1, 0.1 },
                { 0.2, 0.1, 0.3, 0.4 },

                { 0.1, 0.4, 0.2, 0.3 },
                { 0.5, 0.3, 0.1, 0.2 },
                { 0.2, 0.6, 0.1, 0.1 },
                { 0.2, 0.1, 0.3, 0.4 },

                { 0.1, 0.4, 0.2, 0.3 },
                { 0.5, 0.3, 0.1, 0.1 },
                { 0.2, 0.6, 0.1, 0.1 },
                { 0.2, 0.1, 0.3, 0.4 },

                { 0.1, 0.4, 0.2, 0.3 },
                { 0.5, 0.3, 0.1, 0.1 },
                { 0.2, 0.6, 0.1, 0.1 },
                { 0.2, 0.1, 0.3, 0.4 },

                { 0.1, 0.4, 0.2, 0.3 },
                { 0.5, 0.3, 0.1, 0.1 },
                { 0.2, 0.6, 0.1, 0.1 }
            }; // num of row: output length = 19

            double[] initial = { 0.2, 0.25, 0.35, 0.2 };

            HiddenMarkovModel hmm = new HiddenMarkovModel(transition, emission, initial);
            // seqeunce length must match that of emission
            double logLikeliHood = hmm.Evaluate(sequence);

            // At this point, the log-likelihood of the sequence
            // occurring within the model is -3.3928721329161653.
            probability += logLikeliHood;
        }
        static double chain()
        {
            int a = 100000;
            int b = 1099999;
            using StreamReader reader = new(@"S." + a + "_" + b + ".txt");
            int current, next;
            double prob = 0.0F;
            current = reader.Read();
            next = reader.Peek();
            while( ( next = reader.Peek() ) != -1 )
            {
                prob += Math.Log2(transition[current - '0' , next - '0']);
#if LOGFLOAT
                Console.WriteLine(
                    nameof(current) + (current - '0') +
                    nameof(next) + (next - '0') +
                    nameof(transition) + (decimal)transition[current - '0', next - '0']
                    );
#endif
                current = reader.Read();
            } 
            Debug.WriteLine(prob);
            return prob;
        }
    }
}
