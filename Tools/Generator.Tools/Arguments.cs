using System;
using System.Collections.Generic;
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

            // Read arguments
            this.ConnectionString = cmdLine.GetValue("ConnectionString", "cs");
            this.Provider = cmdLine.GetValue("Provider", "p") ?? "SqlServer";
            this.Output = cmdLine.GetValue("Output", "o");
            this.Language = cmdLine.GetValue("Language", "l") ?? "CSharp";
            this.Namespace = cmdLine.GetValue("Namespace", "ns") ?? "Entities";
            this.ClassFormat = cmdLine.GetValue("ClassFormat", "cf") ?? "NameOnly";
            this.ColumnAttribute = cmdLine.GetValue("Attribute", "a");
            this.OnlySchema = cmdLine.GetValue("OnlySchema", "os");
            this.CodeAnalysis = cmdLine.GetValue("CodeAnalysis", "ca");
            this.Source = cmdLine.GetValue("Source", "s");
            this.Separator = cmdLine.GetValue("Separator", "sp");

            // Default
            if (String.IsNullOrEmpty(this.Output) && this.Command == ArgumentCommand.GenerateEntities)
                this.Output = "Entities.cs";

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
        public string Output { get; private set; }
        public string Language { get; private set; }
        public string Namespace { get; private set; }
        public string ClassFormat { get; private set; }
        public string ColumnAttribute { get; private set; }
        public string OnlySchema { get; private set; }
        public string CodeAnalysis { get; private set; }
        public string Source { get; private set; }
        public string Separator { get; private set; }
    }

    public enum ArgumentCommand
    {
        None,
        GenerateEntities,
        Merge
    }
}
