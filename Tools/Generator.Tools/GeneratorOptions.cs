using System;
using System.Data.Common;

namespace Apps72.Dev.Data.Generator.Tools
{
    /// <summary>
    /// Options to use with the <see cref="Generator"/>.
    /// </summary>
    public class GeneratorOptions
    {
        /// <summary>
        /// Commands to execute before the generation of entities.
        /// </summary>
        public Action<DbConnection> PreCommand { get; set; }

        /// <summary>
        /// Commands to execute after the generation of entities.
        /// </summary>
        public Action<DbConnection> PostCommand { get; set; }
    }
}
