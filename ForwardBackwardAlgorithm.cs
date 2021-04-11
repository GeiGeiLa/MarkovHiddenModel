using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromosomeExtraction
{
    public class ForwardBackwardAlgorithm
    {
        /// <summary>
        /// Compute forward probabilities for a given hidden Markov model and a set of observations with scaling
        /// </summary>
        /// <param name="A">Transition Matrix: A[i, j] is the probability of transitioning state i to state j</param>
        /// <param name="B">Emission Matrix: B[observation[t], i] is the probability that given the state at t is i, the observed state at t is observation[t]</param>
        /// <param name="pi">State Vector: pi[i] is the probability that a particular state is t at any time, this can be also interpreted as the probability of initial states </param>
        /// <param name="observations">Observed time series</param>
        /// <param name="scale_vector">Scale Vector</param>
        /// <param name="fwd">Forward Probability Matrix: fwd[t, i] is the scaled probability that provides us with the probability of being in state i at time t.</param>
        public static void Forward(double[,] A, double[,] B, double[] pi, int[] observations, double[] scale_vector, double[,] fwd)
        {
            int T = observations.Length; // length of the observation
            int N = pi.Length; // number of states



            System.Array.Clear(fwd, 0, fwd.Length);

            double c_t = 0.0;

            for (int i = 0; i < N; ++i)
            {
                c_t += fwd[0, i] = pi[i] * B[i, observations[0]];
            }

            //scale probability
            if (c_t != 0)
            {
                for (int i = 0; i < N; ++i)
                {
                    fwd[0, i] /= c_t;
                }
            }

            for (int t = 1; t < T; ++t)
            {
                c_t = 0.0;
                int obs_t = observations[t];

                for (int i = 0; i < N; ++i)
                {
                    double prob_state_i = 0.0; //probability that the sequence will have state at time t equal to i
                    for (int j = 0; j < N; ++j)
                    {
                        prob_state_i += fwd[t - 1, j] * A[j, i];
                    }
                    double prob_obs_state_i = prob_state_i * B[i, obs_t]; //probability that the sequence will have the observed state at time time equal to i
                    fwd[t, i] = prob_obs_state_i;
                    c_t += prob_obs_state_i;
                }

                scale_vector[t] = c_t;

                //scale probability 
                if (c_t != 0)
                {
                    for (int i = 0; i < N; ++i)
                    {
                        fwd[t, i] /= c_t;
                    }
                }
            }
        }

        /// <summary>
        /// Compute forward probabilities for a given hidden Markov model and a set of observations without scaling
        /// </summary>
        /// <param name="A">Transition Matrix: A[i, j] is the probability of transitioning state i to state j</param>
        /// <param name="B">Emission Matrix: B[observation[t], i] is the probability that given the state at t is i, the observed state at t is observation[t]</param>
        /// <param name="pi">State Vector: pi[i] is the probability that a particular state is t at any time, this can be also interpreted as the probability of initial states </param>
        /// <param name="observations">Observed time series</param>
        /// <param name="fwd">Forward Probability Matrix: fwd[t, i] is the scaled probability that provides us with the probability of being in state i at time t.</param>
        public static double[,] Forward(double[,] A, double[,] B, double[] pi, int[] observations)
        {
            int T = observations.Length; // length of the observation
            int N = pi.Length; // number of states

            double[,] fwd = new double[T, N];

            for (int i = 0; i < N; ++i)
            {
                fwd[0, i] = pi[i] * B[i, observations[0]];
            }

            for (int t = 1; t < T; ++t)
            {
                int obs_t = observations[t];

                for (int i = 0; i < N; ++i)
                {
                    double sum = 0.0; //probability that the sequence will have state at time t equal to i
                    for (int j = 0; j < N; ++j)
                    {
                        sum += fwd[t - 1, j] * A[j, i];
                    }
                    double prob_obs_state_i = sum * B[i, obs_t]; //probability that the sequence will have the observed state at time time equal to i
                    fwd[t, i] = prob_obs_state_i;
                }
            }

            return fwd;
        }

        /// <summary>
        /// Compute backward probabilities for a given hidden Markov model and a set of observations with scaling
        /// </summary>
        /// <param name="A">Transition Matrix: A[i, j] is the probability of transitioning state i to state j</param>
        /// <param name="B">Emission Matrix: B[observation[t], i] is the probability that given the state at t is i, the observed state at t is observation[t]</param>
        /// <param name="pi">State Vector: pi[i] is the probability that a particular state is t at any time, this can be also interpreted as the probability of initial states </param>
        /// <param name="observations">Observed time series</param>
        /// <param name="scale_vector">Scale Vector</param>
        /// <param name="bwd">Backward Probability Matrix: bwd[t, i] is the scaled probability that provides us with the probability of being in state i at time t.</param>
        public static void Backward(double[,] A, double[,] B, double[] pi, int[] observations, double[] scale_vector, double[,] bwd)
        {
            int T = observations.Length; //length of time series
            int N = pi.Length; //number of states

            Array.Clear(bwd, 0, bwd.Length);

            for (int i = 0; i < N; ++N)
            {
                bwd[T - 1, i] = 1.0 / scale_vector[T - 1];
            }

            for (int t = T - 2; t >= 0; t--)
            {
                for (int i = 0; i < N; ++i)
                {
                    double sum = 0.0; //probability that the sequence will have state i at time t
                    for (int j = 0; j < N; ++j)
                    {
                        sum += A[i, j] * B[j, observations[t + 1]] * bwd[t + 1, j];
                    }
                    bwd[t, i] += sum / scale_vector[t];
                }
            }
        }

        public static void LogForward(double[,] logA, double[,] logB, double[] logPi, int[] observations, double[,] lnfwd)
        {
            int T = observations.Length; // length of the observation
            int N = logPi.Length; // number of states

            System.Array.Clear(lnfwd, 0, lnfwd.Length);

            for (int i = 0; i < N; ++i)
            {
                lnfwd[0, i] = logPi[i] + logB[i, observations[0]];
            }

            for (int t = 1; t < T; ++t)
            {
                int obs_t = observations[t];

                for (int i = 0; i < N; ++i)
                {
                    double sum = double.NegativeInfinity;
                    for (int j = 0; j < N; ++j)
                    {
                        sum = LogHelper.LogSum(sum, lnfwd[t - 1, j] + logA[j, i]);
                    }
                    lnfwd[t, i] = sum + logB[i, obs_t];
                }
            }
        }

        /// <summary>
        /// Compute forward probabilities for a given hidden Markov model and a set of observations without scaling
        /// </summary>
        /// <param name="logA">Transition Matrix: A[i, j] is the probability of transitioning state i to state j</param>
        /// <param name="logB">Emission Matrix: B[observation[t], i] is the probability that given the state at t is i, the observed state at t is observation[t]</param>
        /// <param name="logPi">State Vector: pi[i] is the probability that a particular state is t at any time, this can be also interpreted as the probability of initial states </param>
        /// <param name="observations">Observed time series</param>
        /// <param name="logLikelihood">The likelihood of the observed time series based on the given hidden Markov model (in log term)</param>
        /// <returns>Forward Probability Matrix: lnfwd[t, i] is the scaled probability that provides us with the probability of being in state i at time t (in log term)</returns>
        public static double[,] LogForward(double[,] logA, double[,] logB, double[] logPi, int[] observations, out double logLikelihood)
        {
            int T = observations.Length; // time series length
            int N = logPi.Length; // number of states

            double[,] lnfwd = new double[T, N];

            LogForward(logA, logB, logPi, observations, lnfwd);

            logLikelihood = double.NegativeInfinity;
            for (int i = 0; i < N; ++i)
            {
                logLikelihood = LogHelper.LogSum(logLikelihood, lnfwd[T - 1, i]);
            }

            return lnfwd;
        }
    }
}
