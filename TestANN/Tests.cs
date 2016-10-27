using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TestANN.Network;
using NeuralNetwork.Network;
using Microsoft.Win32;

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
            TestNetworkSin64();
        }
        public static void TestNetworkSin4()
        {
            Network net = new Network(4, 4, 1);
            List<DataSet> dataSets = new List<DataSet>();
            DataSet ds;
            ds = new DataSet(new double[] { 0, 1, 0, -1 }, new double[] { 0 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 1, 0, -1, 0 }, new double[] { 1 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 0, -1, 0, 1 }, new double[] { 0 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { -1, 0, 1, 0 }, new double[] { -1 }); dataSets.Add(ds);
            net.Train(dataSets, 10000);
            for (int i = 0; i < dataSets.Count; i++) {
                double[] outs = net.Compute(dataSets[i].Values);
                for(int j=0;j<dataSets[i].Values.Count();j++)
                    Console.Write("{0} ", dataSets[i].Values[j]);
                Console.WriteLine("-> {0}", outs[0]);
            }
        }
        public static void TestNetworkSin64()
        {
            int n = 64;
            double[] data = new double[n];
            Network net = new Network(4, 4, 1);
            List<DataSet> dataSets = new List<DataSet>();
            DataSet ds;
            ds = new DataSet(new double[] { 0.5, 1, 0.5, 0 }, new double[] { 0.5 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 1, 0.5, 0, 0.5 }, new double[] { 1.0 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 0.5, 0, 0.5, 1 }, new double[] { 0.5 }); dataSets.Add(ds);
            ds = new DataSet(new double[] { 0, 0.5, 1, 0.5 }, new double[] { 0.0 }); dataSets.Add(ds);
            net.Train(dataSets, 10000);
            for (int i = 0; i < dataSets.Count; i++)
            {
                double[] outs = net.Compute(dataSets[i].Values);
                for (int j = 0; j < dataSets[i].Values.Count(); j++)
                    Console.Write("{0} ", dataSets[i].Values[j]);
                Console.WriteLine("-> {0}", outs[0]);
            }
        }

        public static void TestAutoencoder()
        {
            ImageHelper img = new ImageHelper();
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
                img.loadImage(dlg.FileName);

            int w = 10;
            int inputSize = w * w;
            int hiddenSize = 3;// 5;// 10;
            Network net = new Network(inputSize, hiddenSize, inputSize);

            byte[] data = new byte[w * w];

            List<DataSet> dataSets = new List<DataSet>();
            for (int i = 0; i <= img.Width - w; i+=w)
                for (int j = 0; j <= img.Height - w; j+=w)
                {
                    double[] ddata = new double[w * w];
                    // Получить фрейм(кусочек изображения)
                    img.getData(data, i, j, w, w);
                    for (int k = 0; k < data.Count(); k++)
                        ddata[k] = toDouble(data[k]);
                    //Обучить
                    DataSet ds = new DataSet(ddata, ddata);
                    dataSets.Add(ds);
                }
            double err = 9999;// double.MaxValue;
            //double oldErr = err;
            int epoch = 0;
            double minErr = err;
            while (err>0.1)
            {
                net.Train(dataSets, 10);
                //cохранить результат предсказания
                int z = 0;
                err = 0;
                for (int i = 0; i <= img.Width - w; i += w)
                    for (int j = 0; j <= img.Height - w; j += w)
                    {
                        double[] ddata = net.Compute(dataSets[z].Values);
                        err += net.CalculateError(dataSets[z].Targets);
                    }
                err /= dataSets.Count();
                if (err < minErr)
                    minErr = err;
                epoch++;
            }
            // Сохранить
            {
                int z = 0;
                for (int i = 0; i <= img.Width - w; i += w)
                    for (int j = 0; j <= img.Height - w; j += w)
                    {
                        double[] ddata = net.Compute(dataSets[z++].Values);
                        for (int k = 0; k < ddata.Count(); k++)
                            data[k] = toByte(ddata[k]);
                        img.setData(data, i, j, w, w);
                    }
            }
            //xj=wij/sqrt(sum(wij^2))
            double[] sums = new double[net.HiddenLayer.Count()];
            for(int i=0;i<net.HiddenLayer.Count();i++)
            {
                sums[i] = 0;
                var n = net.HiddenLayer[i];
                foreach (var s in n.InputSynapses)
                {
                    sums[i] += s.Weight * s.Weight;
                }
                sums[i] = Math.Sqrt(sums[i]);
            }
            for(int i=0;i<net.HiddenLayer.Count;i++)
            {
                var n = net.HiddenLayer[i];
                for(int j=0;j<n.InputSynapses.Count;j++)
                {
                    var s = n.InputSynapses[j];
                    data[j] = toByte(s.Weight / sums[i]);
                }
                //img.setData(data, (i * w + w) % img.Width, (i * w) / img.Height, w, w);
                img.setData(data, i*w, 0, w, w);
            }
            img.saveImage(dlg.FileName + err.ToString("F2")+"neurons.jpg");
        }
        static byte toByte(double d)
        {
            if (d < 0.0)
                return 0;
            if (d > 1.0)
                return 255;
            return (byte)(d * 255);
        }
        static double toDouble(byte b)
        {
            return b / 255.0;
        }
    }
}
