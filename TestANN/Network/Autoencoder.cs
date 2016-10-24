using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Network
{
    class Autoencoder
    {
        Network nw;
        public Autoencoder(int inputSize, int hiddenSize, double? learnRate = null, double? momentum = null)
        {
           nw = new Network(inputSize, hiddenSize, inputSize, learnRate, momentum);
        }
        public void Train(List<DataSet> dataSets, int numEpochs)
        {

        }
    }
}
