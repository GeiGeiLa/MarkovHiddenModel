using System;
using static System.Math;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ChromosomeExtraction
{
    class HiddenMarkovModel
    {
        protected double[,] mLogTransitionMatrix;
        protected double[,] mLogEmissionMatrix;
        protected double[] mLogProbabilityVector;
        protected int mSymbolCount = 0;
        protected int mStateCount = 0;

        public double[,] LogTransitionMatrix
        {
            get { return mLogTransitionMatrix; }
        }

        public double[,] LogEmissionMatrix
        {
            get { return mLogEmissionMatrix; }
        }

        public double[] LogProbabilityVector
        {
            get { return mLogProbabilityVector; }
        }

        public double[,] TransitionMatrix
        {
            get { return LogHelper.Exp(mLogTransitionMatrix); }
        }

        public double[,] EmissionMatrix
        {
            get { return LogHelper.Exp(mLogEmissionMatrix); }
        }

        public double[] ProbabilityVector
        {
            get { return LogHelper.Exp(mLogProbabilityVector); }
        }

        /// <summary>
        /// The number of states in the hidden Markov model
        /// </summary>
        public int StateCount
        {
            get { return mStateCount; }
        }

        /// <summary>
        /// The size of symbol set used to construct any observation from this model
        /// </summary>
        public int SymbolCount
        {
            get { return mSymbolCount; }
        }

        public HiddenMarkovModel(double[,] A, double[,] B, double[] pi)
        {
            mLogTransitionMatrix = LogHelper.Log(A);
            mLogEmissionMatrix = LogHelper.Log(B);
            mLogProbabilityVector = LogHelper.Log(pi);

            mStateCount = mLogProbabilityVector.Length;
            mSymbolCount = mLogEmissionMatrix.GetLength(1);
        }


        public double Evaluate(int[] sequence)
        {
            ForwardBackwardAlgorithm.LogForward(
                mLogTransitionMatrix, mLogEmissionMatrix, mLogProbabilityVector, 
                sequence, out double logLikelihood);

            return logLikelihood;
        }
    }
}
