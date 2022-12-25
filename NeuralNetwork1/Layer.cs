using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork1
{
    public interface Layer
    {
        Matrix forward(Matrix x);
        Matrix backward(Matrix dout, double lr);
    }
}
