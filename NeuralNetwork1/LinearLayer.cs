using System.Collections.Generic;
using System;

namespace NeuralNetwork1
{
    class LinearLayer : Layer
    {
        int input_size;
        int output_size;

        Matrix weights;
        Matrix velocity = null;
        double[] bvelocity = null;
        double beta = 0.9;
        double[] b;

        Dictionary<string, Matrix> cache;
        public LinearLayer(int input_size, int output_size)
        {
            this.input_size = input_size;
            this.output_size = output_size;
            this.cache = new Dictionary<string, Matrix>();

            this.b = new double[this.output_size];
            init_weights();
        }

        private void init_weights(double scale_factor = 0.001)
        {
            Random random = new Random();
            double[,] w = new double[this.input_size, this.output_size];
            for (int i = 0; i < this.input_size; i++)
            {
                for (int j = 0; j < this.output_size; j++)
                {
                    w[i, j] = random.NextDouble() * (scale_factor + scale_factor) - scale_factor;
                }
            }

            for (int i = 0; i < this.output_size; i++)
            {
                this.b[i] = 0;
            }
            this.weights = new Matrix(w);
        }

        public Matrix forward(Matrix x)
        //returns Wx + b
        {
            this.cache["x"] = x;
            this.cache["W"] = this.weights;
            return x.dot(weights) + b;
        }

        public Matrix backward(Matrix dout, double lr)
        {
            Matrix x = this.cache["x"];
            double[] db = dout.sum(0);
            Matrix dx = dout.dot(this.weights.T());
            Matrix dW = x.T().dot(dout);

            if (this.velocity is null)
            {
                this.velocity = dW;
            }
            else
            {
                this.velocity = this.velocity * this.beta + dW * (1 - this.beta);
            }
            
            this.weights = this.weights - this.velocity * lr;


            if (this.bvelocity is null)
            {
                this.bvelocity = db;
            }
            else
            {
                for (int i = 0; i < this.output_size; i++)
                    this.bvelocity[i] = this.bvelocity[i] * this.beta + db[i] * (1 - this.beta);
            }
            for (int i = 0; i < this.output_size; i++)
            {
                this.b[i] = this.b[i] - this.bvelocity[i] * lr; 
            }

            return dx;
        }
    }
}
