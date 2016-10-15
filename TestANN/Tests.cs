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
        public static void TestBinaryNetwork2()
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
                    d[j] = shortToDouble(data[(j + i) % data.Length]);
                dataNextVal[i] = shortToDouble(data[(j + i) % data.Length]);
            }
            double[] diff = new double[trainingDataList.Count()];
            double[] oldDiff = new double[trainingDataList.Count()];
            for (i = 0; i < trainingDataList.Count; i++)
                oldDiff[i] = 100.0;
            int cnt = 0;
            double oldDiffSum = Double.MaxValue;
            while (true)
            {
                double currentDiffSum = 0.0;
                for (i = 0; i < trainingDataList.Count(); i++)
                {
                    diff[i] = Math.Abs(bn.getNextVal(trainingDataList[i]) - dataNextVal[i]);
                    currentDiffSum += diff[i];
                }
                if (oldDiffSum > currentDiffSum)
                    oldDiffSum = currentDiffSum;
                else // нет улучшения или возникла ошибка переобучения
                    return;
                for (i = 0; i < trainingDataList.Count(); i++)
                    bn.doTraining(trainingDataList[i], dataNextVal[i], 0.5);
                cnt++;
            }
            //wg.Save();
        }
        public static void TestBinaryNetwork3()
        {
            int depth = 2;
            int i, j;
            BinaryNetwork bn = new BinaryNetwork(depth); // 2^2=4 входа

            //Сгенерируем для обучения несколько синусоид с разной фазой
            short[] data = { 0, 32767, 0, -32767 };
            List<double[]> trainingDataList = new List<double[]>();
            double[] dataNextVal = new double[data.Length];
            for (i = 0; i < data.Length; i++)
            {
                trainingDataList.Add(new double[(int)Math.Pow(2, depth)]);
                double[] d = trainingDataList[i];
                for (j = 0; j < d.Length; j++)
                    d[j] = shortToDouble(data[(j + i) % data.Length]);
                dataNextVal[i] = shortToDouble(data[(j + i) % data.Length]);
            }
            double[] diff = new double[trainingDataList.Count()];
            double[] oldDiff = new double[trainingDataList.Count()];
            for (i = 0; i < trainingDataList.Count; i++)
                oldDiff[i] = 100.0;
            int cnt = 0;
            double oldDiffSum = Double.MaxValue;
            while (true)
            {
                double currentDiffSum = 0.0;
                for (i = 0; i < trainingDataList.Count(); i++)
                {
                    diff[i] = Math.Abs(bn.getNextVal(trainingDataList[i]) - dataNextVal[i]);
                    currentDiffSum += diff[i];
                }
                if (oldDiffSum > currentDiffSum)
                    oldDiffSum = currentDiffSum;
                else // нет улучшения или возникла ошибка переобучения
                    return;
                for (i = 0; i < trainingDataList.Count(); i++)
                    bn.doTraining(trainingDataList[i], dataNextVal[i], 0.5);
                cnt++;
            }
            //wg.Save();
        }
        public static void TestBinaryNetwork()
        {
            int depth = 3;
            int i, j;
            BinaryNetwork bn = new BinaryNetwork(depth); // 2^3=8 входа

            //Сгенерируем для обучения несколько синусоид с разной фазой
            short[] data = { 0, 32767, 0, -32767, 0, 32767, 0, -32767 };
            List<double[]> trainingDataList = new List<double[]>();
            double[] dataNextVal = new double[data.Length];
            for (i = 0; i < data.Length; i++)
            {
                trainingDataList.Add(new double[(int)Math.Pow(2, depth)]);
                double[] d = trainingDataList[i];
                for (j = 0; j < d.Length; j++)
                    d[j] = shortToDouble(data[(j + i) % data.Length]);
                dataNextVal[i] = shortToDouble(data[(j + i) % data.Length]);
            }
            double[] diff = new double[trainingDataList.Count()];
            double[] oldDiff = new double[trainingDataList.Count()];
            for (i = 0; i < trainingDataList.Count; i++)
                oldDiff[i] = 100.0;
            int cnt = 0;
            double oldDiffSum = Double.MaxValue;
            while (true)
            {
                double currentDiffSum = 0.0;
                for (i = 0; i < trainingDataList.Count(); i++)
                {
                    diff[i] = Math.Abs(bn.getNextVal(trainingDataList[i]) - dataNextVal[i]);
                    currentDiffSum += diff[i];
                }
                if (oldDiffSum > currentDiffSum)
                    oldDiffSum = currentDiffSum;
                else // нет улучшения или возникла ошибка переобучения
                    return;
                for (i = 0; i < trainingDataList.Count(); i++)
                    bn.doTraining(trainingDataList[i], dataNextVal[i], 0.5);
                cnt++;
            }
            //wg.Save();
        }
        public static void TestNetwork()
        {
            FullConnectedNetwork fcn = new FullConnectedNetwork(new uint[] { 4,4,1 });
            double[][] ivals = new double[][]{
                new double[]{ 0.5, 1, 0.5, 0 },
                new double[]{ 1, 0.5, 0, 0.5 },
                new double[]{ 0.5, 0, 0.5, 1 },
                new double[]{ 0, 0.5, 1, 0.5 } };
            double[][] ovals = new double[][]{
                new double[]{ 0.5 },
                new double[]{ 1.0 },
                new double[]{ 0.5 },
                new double[]{ 0.0 }};
            double[] res = new double[1];
            double[] diff = new double[4];
            double[] oldDiff = new double[4];
            for (int i = 0; i < 4; i++)
                oldDiff[i] = 100.0;
            int cnt = 0;
            double oldDiffSum = Double.MaxValue;
            while (true)
            {
                double currentDiffSum = 0;
                for (int i = 0; i < ivals.Count(); i++)
                {
                    fcn.handle(ivals[i], res);
                    diff[i] = ovals[i][0] - res[0];
                    currentDiffSum += Math.Abs(diff[i]);
                }
                if (oldDiffSum > currentDiffSum)
                    oldDiffSum = currentDiffSum;
                else // нет улучшения или возникла ошибка переобучения
                    return;
                for (int i = 0; i < ivals.Count(); i++)
                    fcn.doTraining(ivals[i], ovals[i]);
                cnt++;
            }
        }
    }
}
