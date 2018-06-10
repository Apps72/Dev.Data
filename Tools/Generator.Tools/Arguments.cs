using System;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Arguments
    {
        public Arguments(string[] args)
        {
            var cmdLine = new CommandLine(args);

            // Read arguments
            this.ConnectionString = cmdLine.GetValue("ConnectionString", "cs");
            this.Provider = cmdLine.GetValue("Provider", "p") ?? "SqlServer";
            this.Output = cmdLine.GetValue("Output", "o") ?? "Entities.cs";
            this.Language = cmdLine.GetValue("Language", "l") ?? "CSharp";
            this.Namespace = cmdLine.GetValue("Namespace", "ns") ?? "Entities";
            this.ClassFormat = cmdLine.GetValue("ClassFormat", "cf") ?? "NameOnly";

            // Validation
            this.Validate();
        }

        private void Validate()
        {
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

        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public string Output { get; set; }
        public string Language { get; set; }
        public string Namespace { get; set; }
        public string ClassFormat { get; set; }
    }
}
