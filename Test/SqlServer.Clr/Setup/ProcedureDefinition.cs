using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.SqlServerClr.Tests
{
    /// <summary>
    /// Definition of a SQL CLR Procedure
    /// </summary>
    public class ProcedureDefinition
    {
        /// <summary>
        /// Initializes a new instance of a Procedure
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        /// <param name="returns"></param>
        public ProcedureDefinition(string name, ProcedureType type, string arguments, string returns)
        {
            this.Name = name;
            this.Type = type;
            switch (type)
            {
                case ProcedureType.Procedure:
                    this.TypeCode = "PC";
                    this.TypeName = "PROCEDURE";
                    break;
                case ProcedureType.FunctionScalar:
                    this.TypeCode = "FS";
                    this.TypeName = "FUNCTION";
                    break;
                case ProcedureType.FunctionTable:
                    this.TypeCode = "FT";
                    this.TypeName = "FUNCTION";
                    break;
            }
            this.Arguments = arguments;
            this.Returns = returns;
        }

        /// <summary>
        /// Initializes a new instance of a Procedure
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public ProcedureDefinition(string name, ProcedureType type) : this(name, type, string.Empty, string.Empty)
        {

        }

        /// <summary>
        /// Initializes a new instance of a Procedure
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="arguments"></param>
        public ProcedureDefinition(string name, ProcedureType type, string arguments) : this(name, type, arguments, string.Empty)
        {

        }

        /// <summary>
        /// Gets the type of procedure
        /// </summary>
        public ProcedureType Type { get; private set; }

        /// <summary>
        /// Gets the type of code for a procedure (PC, FS, FT)
        /// </summary>
        public string TypeCode { get; private set; }

        /// <summary>
        /// Gets the type of procedure (PROCEDURE? FUNCTION)
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets the name of the procedure
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the definition of all arguments used with this procedure
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Gets the definiton of data returns by this procedure
        /// </summary>
        public string Returns { get; private set; }
    }

    /// <summary>
    /// Definition of type of SQL CLR Methods used in these Unit Tests
    /// </summary>
    public enum ProcedureType
    {
        /// <summary />
        Procedure,
        /// <summary />
        FunctionScalar,
        /// <summary />
        FunctionTable
    }
}
