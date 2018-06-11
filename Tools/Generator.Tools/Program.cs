using System;

namespace Apps72.Dev.Data.Generator.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length <= 0 || args[0].IsEqualTo("-h") || args[0].IsEqualTo("--help"))
            {
                Help.DisplayGeneralHelp();
                return;
            }

            try
            {
                var generator = new Generator(args);
                System.IO.File.WriteAllText(generator.Arguments.Output, generator.Code);
                Console.WriteLine($"Entities generated in {generator.Arguments.Output}.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Console.ResetColor();
            }
            
        }
    }
}
