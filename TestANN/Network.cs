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
}
