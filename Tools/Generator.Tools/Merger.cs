using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Merger
    {
        public Merger(Arguments args)
        {
            this.Files = args.GetFilesForSource();
            this.Output = String.IsNullOrEmpty(args.Output) ? null : new FileInfo(args.Output);
            this.Separator = args.Separator;
        }

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

        public IEnumerable<FileInfo> Files { get; private set; }

        public FileInfo Output { get; private set; }

        public string Separator { get; private set; }
    }
}
