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
            int hiddenSize = 10;
            Network net = new Network(inputSize, hiddenSize, inputSize);

            byte[] data = new byte[w * w];
            double[] ddata = new double[w * w];
            //ВАРИАНТ А отлично обучается
            for(int z=0;z<10;z++)
            for (int i = 0; i < img.Width - w; i++)
                for (int j = 0; j < img.Height - w; j++)
                {
                    // Получить фрейм(кусочек изображения)
                    img.getData(data, i, j, w, w);
                    for (int k = 0; k < data.Count(); k++)
                        ddata[k] = toDouble(data[k]);
                    //Обучить
                    List<DataSet> dataSets = new List<DataSet>();
                    DataSet ds = new DataSet(ddata, ddata);
                    dataSets.Add(ds);
                    net.Train(dataSets, 1);
                }
            ////ВАРИАНТ B не обучается
            //List<DataSet> dataSets = new List<DataSet>();
            //for (int i=0;i<img.Width-w;i++)
            //    for(int j = 0; j < img.Height - w; j++)
            //    {
            //        // Получить фрейм(кусочек изображения)
            //        img.getData(data, i, j, w, w);
            //            for (int k = 0; k < data.Count(); k++)
            //                ddata[k] = toDouble(data[k]);
            //        //Обучить
            //        DataSet ds = new DataSet(ddata, ddata);
            //        dataSets.Add(ds);
            //    }
            //net.Train(dataSets, 10);
            // Сохранить
            //xj=wij/sqrt(sum(wij^2))
            double sum = 0.0;
            foreach(var n in net.HiddenLayer)
            {
                foreach(var s in n.InputSynapses)
                {
                    sum += s.Weight * s.Weight;
                }
            }
            sum = Math.Sqrt(sum);
            for(int i=0;i<net.HiddenLayer.Count;i++)
            {
                var n = net.HiddenLayer[i];
                for(int j=0;j<n.InputSynapses.Count;j++)
                {
                    var s = n.InputSynapses[j];
                    data[j] = (byte)toByte(s.Weight / sum * 255);
                }
                //img.setData(data, (i * w + w) % img.Width, (i * w) / img.Height, w, w);
                img.setData(data, i*w, 0, w, w);
            }
            SaveFileDialog sdlg = new SaveFileDialog();
            if (sdlg.ShowDialog() == true) {
                img.saveImage(sdlg.FileName);
            }
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
