using System;
using System.Linq;

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

#if !DEBUG
            try
#endif
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
                        Console.WriteLine($"  {merger.Files.Count()} files merged. {watch.Elapsed.TotalSeconds:0.00} seconds.");
                        break;


                    case ArgumentCommand.Run:
                        Console.WriteLine($"  Execute SQL scripts...");
                        var runner = new Runner(arguments).Start();
                        Console.WriteLine($"  {runner.Files.Count()} files executed. {watch.Elapsed.TotalSeconds:0.00} seconds.");
                        break;

                    default:
                        Help.DisplayGeneralHelp();
                        return;
                }
            }
#if !DEBUG
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null) Console.WriteLine(ex.InnerException.Message);
                Console.WriteLine("Write DbCmd --help for more information.");
                Console.ResetColor();
                Environment.Exit(-1);
            }
#endif
        }

        private static Version GetAssemblyVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return new Version(fvi.FileVersion);
        }
    }
}
