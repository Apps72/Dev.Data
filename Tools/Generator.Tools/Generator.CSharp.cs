using System;
using System.Collections.Generic;
using System.Text;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class GeneratorCSharp
    {
        private SqlEntitiesGenerator _generator;
        private Arguments _arguments;

        public GeneratorCSharp(SqlEntitiesGenerator generator, Arguments arguments)
        {
            _generator = generator;
            _arguments = arguments;
        }

        public string GenerateCodeForEntities()
        {
            var code = new StringBuilder();
            bool tableNameOnly = _arguments.ClassFormat.IsEqualTo("NameOnly");

            code.AppendLine($"// *********************************************");
            code.AppendLine($"// Code Generated with Apps72.Dev.Data.Generator");
            code.AppendLine($"// *********************************************");
            code.AppendLine();
            code.AppendLine($"namespace {_arguments.Namespace}");
            code.AppendLine($"{{");
            code.AppendLine($"using System;");
            code.AppendLine();

            foreach (var entity in _generator.TablesAndViews)
            {
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
            code.AppendLine($"}}");

            return code.ToString();
        }
    }
}
