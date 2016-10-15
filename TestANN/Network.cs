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
            for (int i = 0; i < weights.Count(); i++)
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
        double[] errorVals; // массив ошибок для обучения
        int depth; // высота двоичного дерева, количество входов сети = 2^depth
        int ncount;
        public BinaryNetwork(int depth_)
        {
            depth = depth_;
            ncount = (int)Math.Pow(2, depth + 1);
            //сеть в форме полного двоичного дерева
            weights = new double[ncount];
            values = new double[ncount];
            errorVals = new double[ncount];
            //инициализация сети
            for (int i = 0; i < ncount; i++)
            {
                weights[i] = 0.1;// Rnd.NextDouble() * 0.2;
                values[i] = 0.0;
            }
        }
        double func(double x) { return 1.0 / (1 + Math.Exp(-x)); }
        double dfunc(double x) { return Math.Exp(-x) / Math.Pow((1 + Math.Exp(-x)), 2); }
        /// <summary>
        /// Обучение сети на входных данных
        /// </summary>
        /// <param name="ivals"></param>
        public void doTraining(double[] ivals, double nextVal, double speed = 0.1)
        {
            initInputs(ivals);
            handleNetwork();
            // обратное распространение ошибки
            errorVals[ncount - 2] = nextVal - values[ncount - 2];
            for (int j = ncount - 2, i = j - 1; i > 0; i -= 2, j--)
            {
                errorVals[i] = errorVals[j] * weights[i];
                errorVals[i - 1] = errorVals[j] * weights[j - 1];
            }
            // корректировка весов
            for (int i = 0, j = (int)Math.Pow(2, depth); i < ncount - 2; i += 2, j++)
            {
                weights[i] += speed * errorVals[j] * dfunc(values[i]);
                weights[i + 1] += speed * errorVals[j] * dfunc(values[i + 1]);
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
            for (int i = 0; i < ovals.Count(); i++)
            {
                ovals[i] = handleNetwork();
                for (int j = 1; j < networkInputsCount; j++)
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
            for (int i = 0; i < n; i++)
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
    public class FullConnectedNetwork
    {
        // входной слой - нулевой ("нижний")
        double[][][] weights; // [индекс слоя][индекс нейрона][индекс входа]
        double[][] values; // сумма входов к нейрону [индекс слоя][индекс нейрона]
        double[][] charges; // результат активационной функции нейрона func(values)
        double[][] errorVals;
        /// <param name="eachLayerNeuronCount">количество нейронов в каждом слое начиная со входного слоя</param>
        public FullConnectedNetwork(uint[] eachLayerNeuronCount)
        {
            if (eachLayerNeuronCount.Count() < 2)
                throw new Exception();
            for (int i = 0; i < eachLayerNeuronCount.Count(); i++)
                if (eachLayerNeuronCount[i] == 0)
                    throw new Exception();
            weights = new double[eachLayerNeuronCount.Count() - 1][][];
            values = new double[eachLayerNeuronCount.Count()][];
            charges = new double[eachLayerNeuronCount.Count()][];
            errorVals = new double[eachLayerNeuronCount.Count()][];// fixme: 0-й слой лишний (оставлен для удобства индексации)
            for (int i = 0; i < eachLayerNeuronCount.Count(); i++)
            {
                uint cnt = eachLayerNeuronCount[i];
                if (i > 0)
                {
                    weights[i - 1] = new double[eachLayerNeuronCount[i]][];
                    for (int j = 0; j < weights[i - 1].Count(); j++)
                    {
                        weights[i - 1][j] = new double[eachLayerNeuronCount[i - 1]];
                        for (int k = 0; k < weights[i - 1][j].Count(); k++)
                            weights[i - 1][j][k] = new Random().NextDouble() * 0.5 - 0.5;
                    }
                    errorVals[i] = new double[eachLayerNeuronCount[i]];
                }
                charges[i] = new double[eachLayerNeuronCount[i]];
                values[i] = new double[eachLayerNeuronCount[i]];
            }
        }
        double func(double x) { return 1.0 / (1 + Math.Exp(-x)); }
        double dfunc(double x) { return Math.Exp(-x) / Math.Pow((1 + Math.Exp(-x)), 2); }
        /// <summary>
        /// Обучение сети на входных данных
        /// </summary>
        /// <param name="ivals"></param>
        public void doTraining(double[] ivals, double[] ovals, double speed = 0.1)
        {
            if (ovals.Count() != values.Last().Count())
                throw new Exception();
            initInputs(ivals);
            handleNetwork();
            // обратное распространение ошибки
            // значение ошибки для выходного слоя
            for (int i = values.Count() - 1; i > 0; i--) // для каждого слоя(кроме входного)
            {
                for (int j = 0; j < values[i].Count(); j++) // для каждого нейрона
                {
                    if (i == values.Count() - 1)
                    {
                        errorVals[i][j] = ovals[j] - charges[i][j];
                    }
                    else
                    {
                        errorVals[i][j] = 0.0;
                        for (int k = 0; k < values[i + 1].Count(); k++) // перебор нейронов следующего(сверху) слоя
                            errorVals[i][j] += errorVals[i + 1][k] * weights[i][k][j];
                    }
                }
            }
            // корректировка весов
            for (int i = 1; i < values.Count(); i++) // для каждого слоя нейронов
            {
                for (int j = 0; j < values[i].Count(); j++) // для каждого нейрона слоя
                {
                    for (int k = 0; k < values[i - 1].Count(); k++) // для каждого входа нейрона
                    {
                        weights[i - 1][j][k] += speed * errorVals[i][j] * dfunc(values[i][j]);
                    }
                }
            }
        }
        double handleNetwork()
        {
            int networkInputsCount = values[0].Count();
            for (int i = 1; i < values.Count(); i++) // для каждого слоя нейронов
            {
                for (int j = 0; j < values[i].Count(); j++) // для каждого нейрона слоя
                {
                    values[i][j] = 0.0;
                    for (int k = 0; k < values[i - 1].Count(); k++) // для каждого входа нейрона (перебор нейронов предыдущего слоя)
                        values[i][j] += charges[i - 1][k] * weights[i - 1][j][k];
                    charges[i][j] = func(values[i][j]);
                }
            }
            return charges.Last().Last();
        }
        void initInputs(double[] ivals)
        {
            // Если входных данных меньше входов сети, то на этих входах будет 0
            // Если входных данных больше входов сети, то возьмем "хвост" данные
            int networkInputsCount = values[0].Count();
            int ivalsOffset = 0;
            int networkInputsOffset = 0;
            if (ivals.Count() > networkInputsCount)
                ivalsOffset = ivals.Count() - networkInputsCount;
            else
                networkInputsOffset = networkInputsCount - ivals.Count();
            for (int i = 0; i < networkInputsOffset; i++)
                values[0][i] = charges[0][i] = 0.0;
            for (int i = networkInputsOffset; i < networkInputsCount; i++)
            {
                charges[0][i] = values[0][i] = ivals[i - networkInputsOffset + ivalsOffset];
            }
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
        public void handle(double[] ivals, double[] ovals)
        {
            if (ovals.Count() != values.Last().Count())
                throw new Exception();
            initInputs(ivals);
            handleNetwork();
            for (int i = 0; i < ovals.Count(); i++)
            {
                ovals[i] = values.Last()[i];
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
            for (int i = 0; i < n; i++)
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
