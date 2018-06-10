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
            Console.WriteLine("   --ConnectionString | -cs    Required. Connection string to the database server.");
            Console.WriteLine("                               See https://www.connectionstrings.com");
            Console.WriteLine("   --Provider         | -p     Type of server: SqlServer, Oracle or SqLite.");
            Console.WriteLine("   --Output           | -o     File name where class will be written.");
            Console.WriteLine("   --Language         | -l     Target format: CSharp or TypeScript.");
            Console.WriteLine("   --Namespace        | -ns    Name of the namespace to generate.");
            Console.WriteLine("   --ClassFormat      | -cf    Format of class: NameOnly or SchemaAndName.");
            Console.WriteLine();
            Console.WriteLine("By default, Provider=SqlServer, Output=Entities.cs, Language=CSharp");
            Console.WriteLine("            Namespace=[Empty], ClassFormat=NameOnly");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  SqlCmd -cs=\"Server=localhost;Database=Scott;\" -p=SqlServer ");
        }
    }
}
