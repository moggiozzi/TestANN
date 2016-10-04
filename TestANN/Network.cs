using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestANN.Network
{
    public abstract class Neuron
    {
        public double[] weights;
        public double getValue(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }
        public double getDerivativeValue(double x)
        {
            return Math.Exp(-x) / Math.Pow((1.0 + Math.Exp(-x)), 2);
        }
        public void correctWeights(double[] inputs, double sigma, double speed = 0.1)
        {
            if (weights.Count() != inputs.Count()) throw new Exception("Inputs count error!");
            for(int i = 0; i < weights.Count(); i++)
            {
                weights[i] += speed * sigma * getDerivativeValue(inputs[i]);
            }
        }
    }
    public class BinaryNetwork
    {
        static Random Rnd = new Random();
        double[] weights;
        double[] values;
        int depth;
        int ncount;
        public BinaryNetwork(int depth_)
        {
            depth = depth_;
            ncount = (int)Math.Pow(2, depth+1);
            //сеть в форме полного двоичного дерева
            weights = new double[ncount];
            values = new double[ncount];
            //инициализация сети
            for (int i = 0; i < ncount; i++)
            {
                weights[i] = Rnd.NextDouble() * 0.2;
                values[i] = 0.0;
            }
        }
        public double func(double x) { return 1.0 / (1 + Math.Exp(-x)); }
        public double dfunc(double x) { return Math.Exp(-x) / Math.Pow((1 + Math.Exp(-x)),2); }
        public void training(double[] ivals)
        {
            double x;
            //i первый источник
            //j приемник
            for (int i=0,j=(int)Math.Pow(2,depth); i < ncount-1; i+=2,j++)
            {
                x = values[i] * weights[i] + values[i + 1] * weights[i + 1];
                values[j] = func(x);
            }
            // обратное распространение ошибки
            double[] s = new double[values.Count()];
            s[ncount - 2] = ivals[ncount-1] - values[ncount-2];
            for (int j = ncount-2, i = j - 1; i > 1; i -= 2, j--)
            {
                s[i] = s[j] * weights[i];
                s[i - 1] = s[j] * weights[j - 1];
            }
            // скорректировать веса
            double trainingSpeed = 0.1;
            for (int i = 0, j = (int)Math.Pow(2, depth); i < ncount - 1; i += 2, j++)
            {
                weights[i] += trainingSpeed * s[j] * dfunc(values[i]);
            }
        }
    }
}
