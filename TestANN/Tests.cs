using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TestANN.Network;
using NeuralNetwork.Network;

namespace TestANN
{
    public class Tests
    {
        public const double WAV_MAX_VAL = 32768.0;
        public const double POW_2_15 = 32768.0;
        public const double POW_2_16 = 65536.0;
        static double shortToDouble(short s)
        {
            return (s + POW_2_15) / POW_2_16;
        }
        static short doubleToShort(double d)
        {
            if (d < 0.0)
                return (short)-POW_2_15;
            if (d > 1.0)
                return (short)(POW_2_15 - 1);
            return (short)(d * POW_2_16 - POW_2_15);
        }
        static void shortToDouble(short[] s, double[] d)
        {
            for (int i = 0; i < s.Count(); i++)
                d[i] = shortToDouble(s[i]);
        }
        static void doubleToShort(double[] d, short[] s)
        {
            for (int i = 0; i < s.Count(); i++)
                s[i] = doubleToShort(d[i]);
        }
        public static void TestNetwork()
        {
            TestNetworkSin();
        }
        public static void TestNetworkSin()
        {
            Network net = new Network(4, 4, 1);
            List<DataSet> dataSets = new List<DataSet>();
            DataSet ds;
            ds = new DataSet(new double[] { 0.5, 1, 0.5, 0 }, new double[] { 0.5 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 1, 0.5, 0, 0.5 }, new double[] { 1.0 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 0.5, 0, 0.5, 1 }, new double[] { 0.5 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 0, 0.5, 1, 0.5 }, new double[] { 0.0 }); dataSets.Add(ds);
            net.Train(dataSets, 1000);
            for (int i = 0; i < dataSets.Count; i++) {
                double[] outs = net.Compute(dataSets[i].Values);
                for(int j=0;j<dataSets[i].Values.Count();j++)
                    Console.Write("{0} ", dataSets[i].Values[j]);
                Console.WriteLine("-> {0}", outs[0]);
            }
        }
    }
}
