using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestANN
{
    class TextHelper
    {
        #region Probability
        // Только строчные!
        // Только русский!
        // Только юникод!
        static int SYMBOL_COUNT = 33;
        static char FIRST_SYMBOL = 'а';
        static char LAST_SYMBOL = 'я';
        static bool isLetter(char ch)
        {
            bool res = ch >= FIRST_SYMBOL && ch <= LAST_SYMBOL;
            return res;
        }
        static int getIdxBySymbol(char ch)
        {
            int res = ch - FIRST_SYMBOL;
            if (res < 0 || res > SYMBOL_COUNT)
                throw new Exception();
            return res;
        }
        static char getSymbolByIdx(int idx)
        {
            if (idx >= SYMBOL_COUNT)
                throw new Exception();
            return (char)(FIRST_SYMBOL + idx);
        }

        public static void calcSymbolCombinationsProbability()
        {
            double[,] probs2 = new double[SYMBOL_COUNT, SYMBOL_COUNT];
            double[,,] probs3 = new double[SYMBOL_COUNT, SYMBOL_COUNT, SYMBOL_COUNT];
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != true)
                return;
            string[] lines = System.IO.File.ReadAllLines(dlg.FileName);
            int comb2Cnt = 0;
            int comb3Cnt = 0;
            foreach (var line in lines)
            {
                char prevSym = '.';
                char prevPrevSym = '.';
                foreach (var sym in line)
                {
                    //if (sym == ' ') continue;
                    if (isLetter(sym) && isLetter(prevSym))
                    {
                        ++probs2[getIdxBySymbol(prevSym), getIdxBySymbol(sym)];
                        comb2Cnt++;
                        if (isLetter(prevPrevSym))
                        {
                            ++probs3[getIdxBySymbol(prevPrevSym), getIdxBySymbol(prevSym), getIdxBySymbol(sym)];
                            comb3Cnt++;
                        }
                    }
                    prevPrevSym = prevSym;
                    prevSym = sym;
                }
            }
            for (int i = 0; i < SYMBOL_COUNT; i++)
                for (int j = 0; j < SYMBOL_COUNT; j++)
                {
                    probs2[i, j] /= comb2Cnt;
                    for (int k = 0; k < SYMBOL_COUNT; k++)
                        probs3[i, j, k] /= comb3Cnt;
                }
            SaveFileDialog saveDlg = new SaveFileDialog();
            if (saveDlg.ShowDialog() == true)
            {
                using (StreamWriter outputFile = new StreamWriter(saveDlg.FileName))
                {
                    outputFile.Write('\t');
                    for (int i = 0; i < SYMBOL_COUNT; i++)
                    {
                        outputFile.Write(getSymbolByIdx(i));
                        outputFile.Write('\t');
                    }
                    outputFile.WriteLine();
                    for (int i = 0; i < SYMBOL_COUNT; i++)
                    {
                        outputFile.Write(getSymbolByIdx(i));
                        outputFile.Write('\t');
                        for (int j = 0; j < SYMBOL_COUNT; j++)
                        {
                            outputFile.Write(probs2[i, j]);
                            outputFile.Write('\t');
                        }
                        outputFile.WriteLine();
                    }
                    // Вывести сочетания в порядке убывания
                    while (true)
                    {
                        double maxProb = 0;
                        int maxi = 0, maxj = 0;
                        for (int i = 0; i < SYMBOL_COUNT; i++)
                        {
                            for (int j = 0; j < SYMBOL_COUNT; j++)
                            {
                                if (probs2[i, j] > maxProb)
                                {
                                    maxi = i;
                                    maxj = j;
                                    maxProb = probs2[i, j];
                                    probs2[i, j] = 0;
                                }
                            }
                        }
                        if (maxProb == 0)
                            break;
                        outputFile.Write(getSymbolByIdx(maxi));
                        outputFile.Write(getSymbolByIdx(maxj));
                        outputFile.Write('\t');
                    }
                    outputFile.WriteLine();
                    while (true)
                    {
                        double maxProb = 0;
                        int maxi = 0, maxj = 0, maxk = 0;
                        for (int i = 0; i < SYMBOL_COUNT; i++)
                        {
                            for (int j = 0; j < SYMBOL_COUNT; j++)
                            {
                                for (int k = 0; k < SYMBOL_COUNT; k++)
                                    if (probs3[i, j, k] > maxProb)
                                    {
                                        maxi = i;
                                        maxj = j;
                                        maxk = k;
                                        maxProb = probs3[i, j, k];
                                        probs3[i, j, k] = 0;
                                    }
                            }
                        }
                        if (maxProb == 0)
                            break;
                        outputFile.Write(getSymbolByIdx(maxi));
                        outputFile.Write(getSymbolByIdx(maxj));
                        outputFile.Write(getSymbolByIdx(maxk));
                        outputFile.Write('\t');
                    }
                }
            }
        }
        #endregion
        #region Sounds
        // гласные звонкие: а о э и у ы 
        // твердные: б г д к п т - в з л м н р с ф х
        // они же мягкие: б' г' д' к' п' т' - в' з' л' м' н' р' с' ф' х'
        // ц - ж ш 
        // ч' - ш' й'
        #endregion
    }
}
