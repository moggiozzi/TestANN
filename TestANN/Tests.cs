using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestANN.Network;

namespace TestANN
{
    public class Tests
    {
        public const double WAV_MAX_VAL = 32760.0;
        static void shortToDouble(short[] s, double[] d)
        {
            for (int i = 0; i < s.Count(); i++)
                d[i] = s[i] / WAV_MAX_VAL;
        }
        static void doubleToShort(double[] d, short[] s)
        {
            for (int i = 0; i < s.Count(); i++)
                s[i] = (short)(d[i] * WAV_MAX_VAL);
        }
        public static void TestBinaryNetwork()
        {
            int depth = 5;
            int i, j;
            BinaryNetwork bn = new BinaryNetwork(depth); // 2^5=32 входа

            //Сгенерируем для обучения несколько синусоид с разной фазой
            WaveGenerator wg = new WaveGenerator();
            short[] data = wg.getData();
            List<double[]> trainingDataList = new List<double[]>();
            double[]   dataNextVal = new double[data.Length];
            for (i = 0; i < data.Length; i++)
            {
                trainingDataList.Add(new double[(int)Math.Pow(2, depth)]);
                double[] d = trainingDataList[i];
                for (j = 0; j < d.Length; j++)
                    d[j] = data[(j + i) % data.Length] / WAV_MAX_VAL;
                dataNextVal[i] = data[(j + i) % data.Length] / WAV_MAX_VAL;
            }
            double diff;
            while (true)
            {
                diff = bn.getNextVal(trainingDataList[i]) - dataNextVal[0];
                bn.doTraining(trainingDataList[0], dataNextVal[0]);
            }
            //wg.Save();
        }
    }
}
