using System;
using System.Linq;

namespace Apps72.Dev.Data.Generator.Tools
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            args = new string[]
            {
                @"Merge",
                @"--source=C:\VSO\Bimvest\PublicTenders\Source\Database\Bimvest.PublicTenders.Database\Scripts",
                @"--Separator=GO",
                //@"--output=allScripts.sql"
            };
#endif

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
                var arguments = new Arguments(args);

                switch (arguments.Command)
                {
                    case ArgumentCommand.GenerateEntities:
                        Console.WriteLine($"  Entities generating...");
                        var generator = new Generator(arguments);
                        System.IO.File.WriteAllText(generator.Arguments.Output, generator.Code);
                        Console.WriteLine($"  {generator.EntitiesGenerated.Count()} entities generated in {generator.Arguments.Output}. {watch.Elapsed.TotalSeconds:0.00} seconds.");
                        break;

                    case ArgumentCommand.Merge:
                        Console.WriteLine($"  Merge files...");
                        var merger = new Merger(arguments).Start();
                        //Console.WriteLine($"  {generator.EntitiesGenerated.Count()} entities generated in {generator.Arguments.Output}. {watch.Elapsed.TotalSeconds:0.00} seconds.");
                        break;

                    default:
                        Help.DisplayGeneralHelp();
                        return;
                }
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
