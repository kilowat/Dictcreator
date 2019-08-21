using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core
{
    public class Parser
    {
        public static async Task RunAsync()
        {
            await Task.Run(() => Run());
        }
        public static void Run()
        {
            if (CheckSettings())
            {
                ReadFile();
            }
        }

        private static bool CheckSettings()
        {
            return true;
        }

        private static void ReadFile()
        {
            string[] testArr = new string[]{"run, bus, cat, smoke, home, bottle"};

            for (int i = 0; i < testArr.Length; i++)
            {
                for (int j = 1; j < 11; j++)
                {

                }
            }
        }
    }
}
