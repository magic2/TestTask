using System;
using Ninject;
using TestTask.Business.Interfaces;

namespace TestTask.ConsoleUI
{
    class Program
    {
        static void DisplayUsage()
        {
            Console.WriteLine("Usage:\n<Tool> <Input file path> <Output file path>\n");
        }
        
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                DisplayUsage();
                Console.ReadLine();
                return;
            }

            DateTime startTime = DateTime.Now;
            Console.WriteLine("Started: {0}\n", startTime);

            IKernel kernel = new StandardKernel();
            kernel.Load(("TestTask.*.dll"));
            var processor = kernel.Get<IProcessor>();

            try
            {
                var result = processor.Process(args[0], args[1]);

                Console.WriteLine("Unique words number: {0}", result.WordsCount);
                Console.WriteLine("Maximum number found: {0}", result.MaximumNumber);
            }
            catch (Business.Exceptions.BusinessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            DateTime stopTime = DateTime.Now;
            Console.WriteLine("\nStopped: {0}", stopTime);

            TimeSpan elapsedTime = stopTime - startTime;
            Console.WriteLine("Elapsed: {0}", elapsedTime);
        }
    }
}
