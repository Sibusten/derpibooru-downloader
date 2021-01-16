using System;
using System.Collections.Generic;
using System.Linq;

namespace Sibusten.Philomena.Downloader.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            // Prompt for arguments when debugging if none are given
            if (!args.Any())
            {
                Console.WriteLine("Enter program arguments, one per line. Finish by entering an empty line.");

                List<string> newArgs = new List<string>();
                while (true)
                {
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        break;
                    }

                    newArgs.Add(input);
                }

                args = newArgs.ToArray();
            }
#endif

            Console.WriteLine("Hello World!");

            Class1.HelloWorld();
        }
    }
}
