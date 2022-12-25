using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Accord.Neuro;
using Accord.Neuro.Learning;

namespace AForge.WindowsForms
{
    public class StudentNetwork : BaseNetwork
    {
        int[] structure;
        double lr = 3;
        List<Layer> layers;
        MSELoss loss;
        public Stopwatch stopWatch = new Stopwatch();
        public StudentNetwork(int[] structure)
        {
            this.structure = structure;
            this.layers = new List<Layer>();
            for (int i = 0; i < structure.Length - 2; i++)
            {
                this.layers.Add(new LinearLayer(structure[i], structure[i + 1]));
                this.layers.Add(new LeakyReluLayer());
            }
            this.layers.Add(new LinearLayer(structure[structure.Length - 2], structure[structure.Length - 1]));
            this.layers.Add(new SigmoidLayer());

            this.loss = new MSELoss();
        }

        public Matrix forward(Matrix x)
        {
            Matrix result = x;
            for (int i = 0; i < this.layers.Count; i++)
            {
                result = this.layers[i].forward(result);
            }
            return result;
        }

        public void backward(Matrix dout)
        {
            for (int i = this.layers.Count - 1; i >= 0; i--)
            {
                dout = this.layers[i].backward(dout, this.lr);
            }
        }

        public double Run(Matrix x, Matrix y)
        {
            //x = x.normilize();
            Matrix pred = this.forward(x);
            List<object> res = this.loss.MSE_forward(pred, y);
            double loss = (double)res[0];
            Matrix dX = (Matrix)res[1];

            this.backward(dX);

            return loss;
        }

        public override int Train(Sample sample, double acceptableError, bool parallel)
        {
            int iters = 1;
            double[,] ax = new double[1, sample.input.Length];
            for (int i = 0; i < sample.input.Length; i++)
            {
                ax[0, i] = sample.input[i];
            }
            double[,] ay = new double[1, sample.Output.Length];
            for (int i = 0; i < sample.Output.Length; i++)
            {
                ay[0, i] = sample.Output[i];
            }
            Matrix x = new Matrix(ax);
            Matrix y = new Matrix(ay);
            double loss = this.Run(x, y);
            while (loss > acceptableError)
            {
                loss = this.Run(x, y);
                iters++;
            }

            var pred = this.forward(x);

            return iters;
        }

        public override double TrainOnDataSet(SamplesSet samplesSet, int epochsCount, double acceptableError, bool parallel)
        {
            double[,] inputs = new double[samplesSet.Count,samplesSet[0].input.Length];
            double[,] outputs = new double[samplesSet.Count,samplesSet[0].Output.Length];

            for (int i = 0; i < samplesSet.Count; ++i)
            {
                for (int j = 0; j < samplesSet[i].input.Length; ++j)
                {
                    inputs[i, j] = samplesSet[i].input[j];
                }

                for (int j = 0; j < samplesSet[i].Output.Length; ++j)
                {
                    outputs[i, j] = samplesSet[i].Output[j];
                }
            }
            Matrix x = new Matrix(inputs);
            Matrix y = new Matrix(outputs);

            int epoch_to_run = 0;
            double error = double.PositiveInfinity;

            StreamWriter errorsFile = File.CreateText("errors.csv");

            stopWatch.Restart();

            while (epoch_to_run < epochsCount && error > acceptableError)
            {
                 epoch_to_run++;
                 error = this.Run(x, y);
#if DEBUG
                 errorsFile.WriteLine(error);
#endif
                 OnTrainProgress((epoch_to_run * 1.0) / epochsCount, error, stopWatch.Elapsed);
                Console.WriteLine($"Epoch {epoch_to_run}/{epochsCount}: loss: {error}");

            }

#if DEBUG
            errorsFile.Close();
#endif
            OnTrainProgress(1.0, error, stopWatch.Elapsed);
            Console.WriteLine($"Epoch {epoch_to_run}/{epochsCount}: loss: {error}");

            stopWatch.Stop();

            return error;
        }

        protected override double[] Compute(double[] input)
        {
            double[,] ax = new double[1, input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                ax[0, i] = input[i];
            }

            Matrix x = new Matrix(ax);
            Matrix y = this.forward(x);
            double[] result = new double[y.shape[1]];
            for (int i = 0; i < y.shape[1]; i++)
            {
                result[i] = y.data[0, i];
            }

            return result;
        }
    }
}