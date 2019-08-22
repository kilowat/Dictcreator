using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dictcreator.Core
{
    public class Parser
    {
        private static List<DataFetcher> _dataFetcher;

        static Parser()
        {
            _dataFetcher = new List<DataFetcher>();
            _dataFetcher.Add(new TranscriptionFetcher());
        }

        public static async Task<bool> RunAsync()
        {
            if (CheckSettings())
            {
                await Task.Run(() => Run());
                return true;
            }
            else
            {
                return false;
            }  
        }
        public static void Run()
        {
            ReadWriteFile();
        }

        private static bool CheckSettings()
        {
            return true;
        }

        private static void ReadWriteFile()
        {
            string[] testArr = new string[]{"run", "home", "dog", "task"};

            for (int i = 0; i < testArr.Length; i++)
            {
                WriteIndex(i);

                foreach (DataFetcher fetcher in _dataFetcher)
                {
                    string result = fetcher.GetResult(testArr[i]);
                    MessageBox.Show(result);
                }
            }
        }

        private static void WriteIndex(int index)
        {

        }
    }
}
