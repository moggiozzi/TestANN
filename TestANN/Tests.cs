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
            BinaryNetwork bn = new BinaryNetwork(5);
            WaveGenerator wg = new WaveGenerator();
            short[] data = wg.getData();
            double[] dd = new double[data.Count()];
            shortToDouble(data, dd);

            double[] dd_pred = new double[data.Count()];
            bn.doTraining(dd);
            bn.getNextArray(dd, ref dd_pred);

            doubleToShort(dd, data);
            //wg.Save();
        }
    }
}
