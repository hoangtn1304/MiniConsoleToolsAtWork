using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckFileStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var c = Console.ReadKey();
                if (c.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(Watcher.Content);
                }
            }
        }
    }
}
