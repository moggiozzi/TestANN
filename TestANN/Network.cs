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
        double[] weights; // веса связей
        double[] values; // значения функции нейрона
        double[] s; // массив ошибок для обучения
        double trainingSpeed = 0.1;
        int depth; // высота двоичного дерева, количество входов сети = 2^depth
        int ncount;
        public BinaryNetwork(int depth_)
        {
            depth = depth_;
            ncount = (int)Math.Pow(2, depth+1);
            //сеть в форме полного двоичного дерева
            weights = new double[ncount];
            values = new double[ncount];
            s = new double[ncount];
            //инициализация сети
            for (int i = 0; i < ncount; i++)
            {
                weights[i] = Rnd.NextDouble() * 0.2;
                values[i] = 0.0;
            }
        }
        double func(double x) { return 1.0 / (1 + Math.Exp(-x)); }
        double dfunc(double x) { return Math.Exp(-x) / Math.Pow((1 + Math.Exp(-x)),2); }
        /// <summary>
        /// Обучение сети на входных данных
        /// </summary>
        /// <param name="ivals"></param>
        public void doTraining(double[] ivals, double nextVal)
        {
            initInputs(ivals);
            handleNetwork();
            // обратное распространение ошибки
            s[ncount - 2] = nextVal - values[ncount-2];
            for (int j = ncount-2, i = j - 1; i > 0; i -= 2, j--)
            {
                s[i] = s[j] * weights[i];
                s[i - 1] = s[j] * weights[j - 1];
            }
            // корректировка весов
            for (int i = 0, j = (int)Math.Pow(2, depth); i < ncount - 2; i += 2, j++)
            {
                weights[i] += trainingSpeed * s[j] * dfunc(values[i]);
                weights[i+1] += trainingSpeed * s[j] * dfunc(values[i+1]);
            }
        }
        double handleNetwork()
        {
            double x;
            //i первый источник
            //j приемник
            for (int i = 0, j = (int)Math.Pow(2, depth); i < ncount - 2; i += 2, j++)
            {
                x = values[i] * weights[i] + values[i + 1] * weights[i + 1];
                values[j] = func(x);
            }
            return values[ncount - 2];
        }
        void initInputs(double[] ivals)
        {
            // Если входных данных меньше входов сети, то на этих входах будет 0
            // Если входных данных больше входов сети, то возьмем "хвост"
            int networkInputsCount = (int)Math.Pow(2, depth);
            int ivalsOffset = 0;
            int networkInputsOffset = 0;
            if (ivals.Count() > networkInputsCount)
                ivalsOffset = ivals.Count() - networkInputsCount;
            else
                networkInputsOffset = networkInputsCount - ivals.Count();
            for (int i = 0; i < networkInputsOffset; i++)
                values[i] = 0.0;
            Array.Copy(ivals, ivalsOffset, values, networkInputsOffset, ivals.Count() - ivalsOffset);
        }
        /// <summary>
        /// Предсказать один элемент
        /// </summary>
        public double getNextVal(double[] ivals)
        {
            initInputs(ivals);
            return handleNetwork();
        }
        /// <summary>
        /// Предсказать массив
        /// </summary>
        public void getNextArray(double[] ivals, ref double[] ovals)
        {
            int networkInputsCount = (int)Math.Pow(2, depth);
            initInputs(ivals);
            for(int i = 0; i < ovals.Count(); i++)
            {
                ovals[i] = handleNetwork();
                for(int j = 1; j < networkInputsCount; j++)
                    values[j - 1] = values[j];
                values[networkInputsCount - 1] = ovals[i];
            }
        }
        /// <summary>
        /// Среднеквадратичное отклонение. Вычислить погрешность предсказания для входных данных.
        /// </summary>
        /// <param name="ivals"></param>
        /// <returns></returns>
        double calcErrorValue(double[] ivals, double[] ovals)
        {
            if (ivals.Count() != ovals.Count())
                throw new Exception("Должны быть одной размерности");
            double s = 0.0;
            int n = ivals.Count();
            for(int i = 0; i < n; i++)
            {
                s += Math.Pow(ivals[i] - ovals[i], 2);
            }
            s = Math.Sqrt(s / n);
            return s;
        }

        public void Save()
        {
            //todo
        }
        public bool Load()
        {
            //todo
            return false;
        }
    }
}
