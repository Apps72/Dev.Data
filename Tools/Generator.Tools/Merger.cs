using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data.Generator.Tools
{
    class Merger
    {
        public Merger(Arguments args)
        {
            this.Arguments = args;
            this.Files = GetFiles();
            this.Output = String.IsNullOrEmpty(args.Output) ? null : new FileInfo(args.Output);
            this.Separator = args.Separator;
        }

        public string[] SourceFiles { get; set; }

        public Merger Start()
        {
            if (Output != null && Output.Exists)
                Output.Delete();

            foreach (var file in this.Files.OrderBy(i => i.FullName))
            {
                if (file.Exists)
                {
                    StringBuilder content = new StringBuilder();

                    content.AppendLine($"--------------------------------------------------------------------------------");
                    content.AppendLine($"-- Script {file.Name}");
                    content.AppendLine($"--------------------------------------------------------------------------------");
                    content.AppendLine();
                    content.AppendLine(File.ReadAllText(file.FullName));
                    content.AppendLine();
                    content.AppendLine(Separator);
                    content.AppendLine();

                    if (Output != null)
                    {
                        File.WriteAllText(Output.FullName, content.ToString());
                    }
                    else
                    {
                        Console.WriteLine(content);
                    }
                }
                else
                {
                    Console.WriteLine($"Script \"{file.FullName}\" not found.");
                }
            }

            return this;
        }

        public Arguments Arguments { get; private set; }

        public DirectoryInfo SourceDirectory { get; private set; }

        public string SourcePattern { get; private set; }

        public IEnumerable<FileInfo> Files { get; private set; }

        public FileInfo Output { get; private set; }

        public string Separator { get; private set; }

        private IEnumerable<FileInfo> GetFiles()
        {
            // Source
            string source = Arguments.Source;
            if (String.IsNullOrEmpty(Arguments.Source))
                source = Path.Join(Environment.CurrentDirectory, "*.sql");
            else if (!Wildcard.HasWildcard(source))
                source = Path.Join(source, "*.sql");

            // Directory of Source
            var sourceWithPattern = new FileInfo(source);
            SourceDirectory = sourceWithPattern.Directory;
            SourcePattern = sourceWithPattern.Name;

            var files = new List<FileInfo>();
            var wildcard = new Wildcard(source, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            foreach (var file in SourceDirectory.EnumerateFiles(SourcePattern, SearchOption.AllDirectories))
            {
                if (wildcard.IsMatch(file.FullName))
                    files.Add(file);
            }

            return files;
        }
    }
}
