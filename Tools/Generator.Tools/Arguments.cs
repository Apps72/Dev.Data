using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Arguments
    {
        public Arguments(string[] args)
        {
            var cmdLine = new CommandLine(args);

            // Read command
            this.Command = ArgumentCommand.None;
            if (cmdLine.ContainsKey("GenerateEntities", "ge"))
            {
                this.Command = ArgumentCommand.GenerateEntities;
            }
            else if (cmdLine.ContainsKey("Merge", "mg"))
            {
                this.Command = ArgumentCommand.Merge;
            }
            else if (cmdLine.ContainsKey("Run", "rn"))
            {
                this.Command = ArgumentCommand.Run;
            }

            // Read arguments
            this.ConnectionString = cmdLine.GetValue("ConnectionString", "cs");
            this.Provider = cmdLine.GetValue("Provider", "p") ?? "SqlServer";
            this.Output = cmdLine.GetValue("Output", "o");
            this.Language = cmdLine.GetValue("Language", "l") ?? "CSharp";
            this.Namespace = cmdLine.GetValue("Namespace", "ns") ?? "Entities";
            this.NullableRefTypes = cmdLine.ContainsKey("NullableRefTypes", "nrt");
            this.ValidationAttributes = cmdLine.GetValue("Validations", "val").Split(new[] {',', ';' })?.Select(i => i.Trim().ToLower())?.ToArray();
            this.SortProperties = cmdLine.ContainsKey("SortProperties", "sp");
            this.ClassFormat = cmdLine.GetValue("ClassFormat", "cf") ?? "NameOnly";
            this.ColumnAttribute = cmdLine.GetValue("Attribute", "a");
            this.OnlySchema = cmdLine.GetValue("OnlySchema", "os");
            this.CodeAnalysis = cmdLine.GetValue("CodeAnalysis", "ca");
            this.Source = cmdLine.GetValue("Source", "s");
            this.Separator = cmdLine.GetValue("Separator", "sp") ?? "GO";
            this.DbConfigAfter = cmdLine.GetValue("DbConfigAfter", "ca");
            this.DbConfigUpdate = cmdLine.GetValue("DbConfigUpdate", "cu");

            // Default
            if (String.IsNullOrEmpty(this.Output) && this.Command == ArgumentCommand.GenerateEntities)
                this.Output = "Entities.cs";

            // Source
            if (this.Command == ArgumentCommand.Merge ||
                this.Command == ArgumentCommand.Run)
            {
                if (String.IsNullOrEmpty(this.Source))
                    this.Source = Path.Join(Environment.CurrentDirectory, "*.sql");
                else if (!Wildcard.HasWildcard(this.Source))
                    this.Source = Path.Join(Source, "*.sql");
            }

            // Validation
            this.Validate();
        }

        private void Validate()
        {
            if (this.Command == ArgumentCommand.None)
            {
                throw new ArgumentException("Please, specify a command to execute: GenerateEntities, ...");
            }

            if (this.Provider.IsNotEqualTo("SqlServer") &&
                this.Provider.IsNotEqualTo("Oracle") &&
                this.Provider.IsNotEqualTo("SqLite"))
            {
                throw new ArgumentException("The 'Provider' must be only SqlServer, Oracle or SqLite.");
            }

            if (this.Language.IsNotEqualTo("CSharp") &&
                this.Language.IsNotEqualTo("TypeScript"))
            {
                throw new ArgumentException("The 'Language' must be only CSharp or TypeScript.");
            }

            if (this.ClassFormat.IsNotEqualTo("NameOnly") &&
                this.ClassFormat.IsNotEqualTo("SchemaAndName"))
            {
                throw new ArgumentException("The 'ClassFormat' must be only NameOnly or SchemaAndName.");
            }
        }

        public ArgumentCommand Command { get; private set; }
        public string ConnectionString { get; private set; }
        public string Provider { get; private set; }
        public bool SortProperties { get; set; }
        public string Output { get; private set; }
        public string Language { get; private set; }
        public string Namespace { get; private set; }
        public bool NullableRefTypes { get; set; }
        public string[] ValidationAttributes { get; set; }
        public string ClassFormat { get; private set; }
        public string ColumnAttribute { get; private set; }
        public string OnlySchema { get; private set; }
        public string CodeAnalysis { get; private set; }
        public string Source { get; private set; }
        public string Separator { get; private set; }
        public string DbConfigAfter { get; private set; }
        public string DbConfigUpdate { get; private set; }

        /// <summary>
        /// Gets files from the Source argument, using wildcard.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FileInfo> GetFilesForSource()
        {
            // Directory of Source
            var sourceWithPattern = new FileInfo(this.Source);
            var sourceDirectory = sourceWithPattern.Directory;
            var sourcePattern = sourceWithPattern.Name;

            var files = new List<FileInfo>();
            var wildcard = new Wildcard(this.Source, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            foreach (var file in sourceDirectory.EnumerateFiles(sourcePattern, SearchOption.AllDirectories))
            {
                if (wildcard.IsMatch(file.FullName))
                    files.Add(file);
            }

            return files;
        }
    }

    public enum ArgumentCommand
    {
        None,
        GenerateEntities,
        Merge,
        Run
    }
}
