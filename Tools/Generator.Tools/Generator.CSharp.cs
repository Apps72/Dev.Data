using System;
using System.Collections.Generic;
using System.Linq;
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

            // All tables
            var allTables = _generator.TablesAndViews;
            if (_arguments.SortProperties)
                allTables = allTables.OrderBy(i => i.SchemaAndName);

            // Entities
            foreach (var entity in allTables)
            {
                if (String.IsNullOrEmpty(_arguments.OnlySchema) ||
                    entity.Schema.IsEqualTo(_arguments.OnlySchema))
                {
                    tables.Add(entity);

                    code.AppendLine($"    /// <summary />");
                    code.AppendLine($"    public partial class {(tableNameOnly ? entity.Name : entity.SchemaAndName)}");
                    code.AppendLine($"    {{");

                    var allColumns = entity.Columns;
                    if (_arguments.SortProperties)
                        allColumns = allColumns.OrderBy(i => i.ColumnName).ToArray();

                    foreach (var column in allColumns)
                    {
                        code.AppendLine($"        /// <summary />");

                        if (column.ColumnName.IsNotEqualTo(column.DotNetColumnName))
                        {
                            code.AppendLine($"        [Apps72.Dev.Data.Annotations.Column(\"{column.ColumnName}\")]");

                            if (!String.IsNullOrEmpty(_arguments.ColumnAttribute))
                                code.AppendLine($"        [{_arguments.ColumnAttribute}(\"{column.ColumnName}\")]");
                        }

                        string csharpType = _arguments.NullableRefTypes ? column.CSharp8TypeNullable : column.CSharpTypeNullable;

                        code.AppendLine($"        public virtual {csharpType} {column.DotNetColumnName} {{ get; set; }}");
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
