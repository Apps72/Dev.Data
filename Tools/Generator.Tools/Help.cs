using System;
using System.Collections.Generic;
using System.Text;

namespace Apps72.Dev.Data.Generator.Tools
{
    public class Help
    {
        public static void DisplayGeneralHelp()
        {
            Console.WriteLine("SqlDatabase Command Line Tools (2.5.0)");
            Console.WriteLine(" Usage: SqlCmd [options] ");
            Console.WriteLine();
            Console.WriteLine(" Options:");
            Console.WriteLine("   --ConnectionString | -cs    Connection string to the database server.");
            Console.WriteLine("                               See https://www.connectionstrings.com");
            Console.WriteLine("   --Provider         | -p     Type of server: SqlServer, Oracle of SqLite.");
            Console.WriteLine("   --Output           | -o     File name where class will be written.");
            Console.WriteLine("   --Language         | -f     Target format: CSharp, TypeScript.");
            Console.WriteLine("   --Namespace        | -ns    Name of the namespace to generate.");
            Console.WriteLine("   --EntityFormat     | -t     Format of class: NameOnly, SchemaAndName.");
        }
    }
}
