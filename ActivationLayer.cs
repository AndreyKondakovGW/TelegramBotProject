using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForge.WindowsForms
{
    class SigmoidLayer : Layer
    {
        Dictionary<string, Matrix> cache;
        double alpha = 2.0;
        public SigmoidLayer()
        {
            this.cache = new Dictionary<string, Matrix>();
        }

        private double sigmoid(double x)
        {
            return 1 / (1 + System.Math.Exp(-alpha * x));
        }
        public Matrix forward(Matrix x)
        {
            this.cache["x"] = x;
            double[,] result = new double[x.shape[0], x.shape[1]];
            for (int i = 0; i < x.shape[0]; i++)
            {
                for (int j = 0; j < x.shape[1]; j++)
                {
                    result[i, j] = sigmoid(x.data[i, j]);
                }
            }
            return new Matrix(result);
        }

        public Matrix backward(Matrix dout, double lr)
        {
            Matrix x = this.cache["x"];
            double[,] result = new double[dout.shape[0], dout.shape[1]];
            for (int i = 0; i < dout.shape[0]; i++)
            {
                for (int j = 0; j < dout.shape[1]; j++)
                {
                    result[i, j] = alpha * dout.data[i, j] * sigmoid(x.data[i, j]) * (1 - sigmoid(x.data[i, j]));
                }
            }
            return new Matrix(result);
        }
    }

    class ReluLayer : Layer
    {
        Dictionary<string, Matrix> cache;
        public ReluLayer()
        {
            this.cache = new Dictionary<string, Matrix>();
        }

        public Matrix forward(Matrix x)
        {
            this.cache["x"] = x;
            return x.maxWithInt(0);
        }

        public Matrix backward(Matrix dout, double lr)
        {
            Matrix x = this.cache["x"];
            double[,] result = new double[dout.shape[0], dout.shape[1]];
            for (int i = 0; i < dout.shape[0]; i++)
            {
                for (int j = 0; j < dout.shape[1]; j++)
                {
                    result[i, j] = dout.data[i, j] * (x.data[i, j] > 0 ? 1 : 0);
                }
            }
            return new Matrix(result);
        }
    }

    class LeakyReluLayer : Layer
    {
        Dictionary<string, Matrix> cache;
        double neg_slope = 0.1;
        public LeakyReluLayer()
        {
            this.cache = new Dictionary<string, Matrix>();
        }

        public Matrix forward(Matrix x)
        {
            this.cache["x"] = x;
            for (int i = 0; i < x.shape[0]; i++)
            {
                for (int j = 0; j < x.shape[1]; j++)
                {
                    if (x.data[i, j] < 0)
                    {
                        x.data[i, j] *= neg_slope;
                    }
                }
            }
            return x.maxWithInt(0);
        }

        public Matrix backward(Matrix dout, double lr)
        {
            Matrix x = this.cache["x"];
            double[,] result = new double[dout.shape[0], dout.shape[1]];
            for (int i = 0; i < dout.shape[0]; i++)
            {
                for (int j = 0; j < dout.shape[1]; j++)
                {
                    result[i, j] = dout.data[i, j] * (x.data[i, j] > 0 ? 1 : neg_slope);
                }
            }
            return new Matrix(result);
        }
    }

    class SoftmaxLayer : Layer
    {
        Dictionary<string, Matrix> cache;
        public SoftmaxLayer()
        {
            this.cache = new Dictionary<string, Matrix>();
        }

        public Matrix forward(Matrix x)
        {
            this.cache["x"] = x;
            double[,] result = new double[x.shape[0], x.shape[1]];
            for (int i = 0; i < x.shape[0]; i++)
            {
                double sum = 0;
                for (int j = 0; j < x.shape[1]; j++)
                {
                    sum += System.Math.Exp(x.data[i, j]);
                }
                for (int j = 0; j < x.shape[1]; j++)
                {
                    result[i, j] = System.Math.Exp(x.data[i, j]) / sum;
                }
            }
            return new Matrix(result);
        }

        public Matrix backward(Matrix dout, double lr)
        {
            Matrix x = this.cache["x"];
            double[,] result = new double[dout.shape[0], dout.shape[1]];
            for (int i = 0; i < dout.shape[0]; i++)
            {
                double sum = 0;
                for (int j = 0; j < dout.shape[1]; j++)
                {
                    sum += System.Math.Exp(x.data[i, j]);
                }
                for (int j = 0; j < dout.shape[1]; j++)
                {
                    result[i, j] = System.Math.Exp(x.data[i, j]) / sum * (1 - System.Math.Exp(x.data[i, j]) / sum);
                }
            }
            return new Matrix(result);
        }
    }
}
