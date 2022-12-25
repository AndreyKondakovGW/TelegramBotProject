using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AForge.WindowsForms
{
    class MSELoss
    {
        public List<object> MSE_forward(Matrix x, Matrix y)
        //y  - метки классов в батче
        {
            double loss = 0;
            double[,] dx = new double[x.shape[0], x.shape[1]];

            for (int i = 0; i < x.shape[0]; i++)
            {
                for (int j = 0; j < x.shape[1]; j++)
                {
                    loss += System.Math.Pow(x.data[i, j] - y.data[i, j], 2) / x.shape[1];
                    dx[i, j] = 2 * (x.data[i, j] - y.data[i, j]) / x.shape[1];
                }
            }
            //loss /= x.shape[0];
            Matrix dX = new Matrix(dx);
            dX = dX.divide(x.shape[0]);
            return new List<object> { loss, dX };
        }
    }
}
