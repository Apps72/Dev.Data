using System;
using System.Collections.Generic;
using System.Text;

namespace Apps72.Dev.Data.Generator.Tools
{
    internal class GeneratorCSharp
    {
        private EntityGenerator _generator;
        private Arguments _arguments;

        public GeneratorCSharp(EntityGenerator generator, Arguments arguments)
        {
            _generator = generator;
            _arguments = arguments;

            this.Start();
        }

        public string Code { get; private set; }

        public IEnumerable<Schema.DataTable> Entities { get; set; }

        private string Start()
        {
            var code = new StringBuilder();
            List<Schema.DataTable> tables = new List<Schema.DataTable>();

            bool tableNameOnly = _arguments.ClassFormat.IsEqualTo("NameOnly");

            code.AppendLine($"/* *********************************************");
            code.AppendLine($"   Code Generated with Apps72.Dev.Data.Generator");
            code.AppendLine($"   *********************************************");
            code.AppendLine($"   $ dotnet tool install -g Apps72.Dev.Data.Generator.Tools ");
            code.AppendLine($"   $ DbCmd --Help ");
            code.AppendLine($"*/");
            code.AppendLine();
            code.AppendLine($"namespace {_arguments.Namespace}");
            code.AppendLine($"{{");

            // Pragma       
            if (!String.IsNullOrEmpty(_arguments.CodeAnalysis))
                code.AppendLine($"    #pragma warning disable {_arguments.CodeAnalysis} ");

            // Namespace
            code.AppendLine($"    using System;");
            code.AppendLine();

            // Entities
            foreach (var entity in _generator.TablesAndViews)
            {
                if (String.IsNullOrEmpty(_arguments.OnlySchema) ||
                    entity.Schema.IsEqualTo(_arguments.OnlySchema))
                {
                    tables.Add(entity);

                    code.AppendLine($"    /// <summary />");
                    code.AppendLine($"    public partial class {(tableNameOnly ? entity.Name : entity.SchemaAndName)}");
                    code.AppendLine($"    {{");

                    foreach (var column in entity.Columns)
                    {
                        code.AppendLine($"        /// <summary />");

                        if (column.ColumnName.IsNotEqualTo(column.DotNetColumnName))
                        {
                            code.AppendLine($"        [Apps72.Dev.Data.Annotations.Column(\"{column.ColumnName}\")]");

                            if (!String.IsNullOrEmpty(_arguments.ColumnAttribute))
                                code.AppendLine($"        [{_arguments.ColumnAttribute}(\"{column.ColumnName}\")]");
                        }

                        code.AppendLine($"        public virtual {column.CSharpTypeNullable} {column.DotNetColumnName} {{ get; set; }}");
                    }

                    code.AppendLine($"    }}");
                }
            }
            code.AppendLine($"}}");

            // Pragma       
            if (!String.IsNullOrEmpty(_arguments.CodeAnalysis))
                code.AppendLine($"#pragma warning restore {_arguments.CodeAnalysis} ");


            this.Entities = tables;
            this.Code = code.ToString();

            return this.Code;
        }
    }
}
