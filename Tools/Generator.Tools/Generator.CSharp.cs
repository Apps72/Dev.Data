using Apps72.Dev.Data.Convertor;
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
            if (_arguments.ValidationAttributes)
            {
                code.AppendLine($"    using System.ComponentModel.DataAnnotations;");
            }
            code.AppendLine();

            // All tables
            var allTables = _generator.TablesAndViews;
            if (_arguments.SortProperties)
                allTables = allTables.OrderBy(i => i.SchemaAndName);

            // Entities
            string onlySchema = Apps72.Dev.Data.Convertor.DataTableConvertor.RemoveExtraChars(_arguments.OnlySchema);
            foreach (var entity in allTables)
            {
                if (String.IsNullOrEmpty(onlySchema) ||
                    entity.Schema.IsEqualTo(onlySchema))
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
                        string defaultValue = _arguments.NullableRefTypes && csharpType == "string" ? " = string.Empty;" : String.Empty;

                        // Validation
                        if (_arguments.ValidationAttributes)
                        {
                            // [StringLength(...)]
                            if (column.DataType == typeof(String) &&
                                column.Size > 0)
                            {
                                code.AppendLine($"        [StringLength({column.Size})]");
                            }

                            // [Range(..., ...)]
                            if (column.DataType == typeof(Decimal) ||
                                column.DataType == typeof(Double) ||
                                column.DataType == typeof(Single))
                            {
                                var minMax = column.GetMinMax();
                                code.AppendLine($"        [Range({minMax.Item1}d, {minMax.Item2}d)]");
                            }
                        }

                        code.AppendLine($"        public virtual {csharpType} {column.DotNetColumnName} {{ get; set; }}{defaultValue}");
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
