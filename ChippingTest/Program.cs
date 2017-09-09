using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChippingTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("> ");
                string readed = Console.ReadLine();
                string chip = Symphony.Util.StringCipher.Encrypt(readed, "helloWorld7.&GG");
                Clipboard.SetText(chip);
                Console.WriteLine(chip);
            }
        }
    }
}
