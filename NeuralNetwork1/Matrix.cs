using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork1
{
    public class Matrix
    {
        public double[,] data;
        public int[] shape;

        public Matrix(double[,] d)
        {
            data = d;
            shape = new int[2];
            shape[0] = d.GetLength(0);
            shape[1] = d.GetLength(1);
        }

        public Matrix dot(Matrix other)
        {
            // if (this.shape[1] != other.shape[0])
            // {
            //     throw new Exception("Matrix dimensions must agree");
            // }
            // double[,] result = new double[this.shape[0], other.shape[1]];
            // for (int i = 0; i < this.shape[0]; i++)
            // {
            //     for (int j = 0; j < other.shape[1]; j++)
            //     {
            //         for (int k = 0; k < this.shape[1]; k++)
            //         {
            //             result[i, j] += data[i, k] * other.data[k, j];
            //         }
            //     }
            // }
            /* return new Matrix(result); */

            if (this.shape[1] != other.shape[0])
            {
                throw new Exception("Matrix dimensions must agree");
            }
            var res = new double[this.shape[0], other.shape[1]];
            Parallel.For(0, this.shape[0], i =>
            {
                for (var j = 0; j < other.shape[1]; j++) 
                {
                    for (var k = 0; k < this.shape[1]; k++)
                    {
                        res[i, j] += data[i, k] * other.data[k, j];
                    }
                }
            });
            return new Matrix(res);
        }

        public Matrix maxWithInt(int a)
        {
            double[,] result = new double[this.shape[0], this.shape[1]];
            for (int i = 0; i < this.shape[0]; i++)
            {
                for (int j = 0; j < this.shape[1]; j++)
                {
                    result[i, j] = System.Math.Max(data[i, j], a);
                }
            }
            return new Matrix(result);
        }

        public Matrix T()
        {
            double[,] result = new double[this.shape[1], this.shape[0]];
            for (int i = 0; i < this.shape[0]; i++)
            {
                for (int j = 0; j < this.shape[1]; j++)
                {
                    result[j, i] = data[i, j];
                }
            }
            return new Matrix(result);
        }

        public Matrix divide(double a)
        {
            double[,] result = new double[this.shape[0], this.shape[1]];
            for (int i = 0; i < this.shape[0]; i++)
            {
                for (int j = 0; j < this.shape[1]; j++)
                {
                    result[i, j] = data[i, j] / a;
                }
            }
            return new Matrix(result);
        }

        public double[] sum(int axis)
        {
            if (axis == 0)
            {
                double[] result = new double[this.shape[1]];
                for (int i = 0; i < this.shape[1]; i++)
                {
                    for (int j = 0; j < this.shape[0]; j++)
                    {
                        result[i] += data[j, i];
                    }
                }
                return result;
            }
            else if (axis == 1)
            {
                double[] result = new double[this.shape[0]];
                for (int i = 0; i < this.shape[0]; i++)
                {
                    for (int j = 0; j < this.shape[1]; j++)
                    {
                        result[i] += data[i, j];
                    }
                }
                return result;
            }
            else
            {
                throw new Exception("Axis must be 0 or 1");
            }
        }

        public Matrix normilize()
        {
            double[,] result = new double[this.shape[0], this.shape[1]];
            double mean = 0;
            double std = 0;
            for (int i = 0; i < this.shape[0]; i++)
            {
                for (int j = 0; j < this.shape[1]; j++)
                {
                    mean += data[i, j];
                }
            }

            mean /= this.shape[0] * this.shape[1];

            for (int i = 0; i < this.shape[0]; i++)
            {
                for (int j = 0; j < this.shape[1]; j++)
                {
                    std += (data[i, j] - mean) * (data[i, j] - mean);
                }
            }
            std /= this.shape[0] * this.shape[1];
            std = System.Math.Sqrt(std);

            for (int i = 0; i < this.shape[0]; i++)
            {
                for (int j = 0; j < this.shape[1]; j++)
                {
                    result[i, j] = (data[i, j] - mean) / std;
                }
            }
            return new Matrix(result);
        }
        public static Matrix operator +(Matrix a, double[] b)
        {
            if (a.shape[0] == b.Length)
            {
                double[,] result = new double[a.shape[0], a.shape[1]];
                for (int i = 0; i < a.shape[0]; i++)
                {
                    for (int j = 0; j < a.shape[1]; j++)
                    {
                        result[i, j] = a.data[i, j] + b[i];
                    }
                }
                return new Matrix(result);
            }
            else if (a.shape[1] == b.Length)
            {
                double[,] result = new double[a.shape[0], a.shape[1]];
                for (int i = 0; i < a.shape[0]; i++)
                {
                    for (int j = 0; j < a.shape[1]; j++)
                    {
                        result[i, j] = a.data[i, j] + b[j];
                    }
                }
                return new Matrix(result);
            }
            else
            {
                throw new Exception("Matrix dimensions must agree");
            }
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.shape[0] != b.shape[0] || a.shape[1] != b.shape[1])
            {
                throw new Exception("Matrix dimensions must agree");
            }
            double[,] result = new double[a.shape[0], a.shape[1]];
            for (int i = 0; i < a.shape[0]; i++)
            {
                for (int j = 0; j < a.shape[1]; j++)
                {
                    result[i, j] = a.data[i, j] + b.data[i, j];
                }
            }
            return new Matrix(result);
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.shape[0] != b.shape[0] || a.shape[1] != b.shape[1])
            {
                throw new Exception("Matrix dimensions must agree");
            }
            double[,] result = new double[a.shape[0], a.shape[1]];
            for (int i = 0; i < a.shape[0]; i++)
            {
                for (int j = 0; j < a.shape[1]; j++)
                {
                    result[i, j] = a.data[i, j] - b.data[i, j];
                }
            }
            return new Matrix(result);
        }

        public static Matrix operator *(Matrix a, double b)
        {
            double[,] result = new double[a.shape[0], a.shape[1]];
            for (int i = 0; i < a.shape[0]; i++)
            {
                for (int j = 0; j < a.shape[1]; j++)
                {
                    result[i, j] = a.data[i, j] * b;
                }
            }
            return new Matrix(result);
        }

    }
}
