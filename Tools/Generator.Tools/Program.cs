using System;

namespace Apps72.Dev.Data.Generator.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"SqlDatabase Command Line Tools (v{GetAssemblyVersion().ToString(3)})");
            Console.WriteLine($"Project on https://github.com/Apps72/Dev.Data");

            if (args == null || args.Length <= 0 || args[0].IsEqualTo("-h") || args[0].IsEqualTo("--help"))
            {
                Help.DisplayGeneralHelp();
                return;
            }

            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Console.WriteLine($"  Entities generating...");
                var generator = new Generator(args);
                System.IO.File.WriteAllText(generator.Arguments.Output, generator.Code);
                Console.WriteLine($"  Entities generated in {generator.Arguments.Output}. {watch.Elapsed.TotalSeconds:0.00} seconds.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine("Write DbCmd --help for more information.");
                Console.ResetColor();
            }
            
        }

        private static Version GetAssemblyVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(fvi.FileVersion);
        }
    }
}
