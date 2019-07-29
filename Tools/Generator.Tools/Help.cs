using System;

namespace Apps72.Dev.Data.Generator.Tools
{
    internal class Help
    {
        public static void DisplayGeneralHelp()
        {
            Console.WriteLine(" Usage: DbCmd <command> [options] ");
            Console.WriteLine();
            Console.WriteLine(" Commands:");
            Console.WriteLine("   GenerateEntities   | ge     Generate a file (see --Output) with all entities");
            Console.WriteLine("                               extracted from tables and view of specified database.");
            Console.WriteLine("   Merge              | mg     ");
            Console.WriteLine("                               ");
            Console.WriteLine();
            Console.WriteLine(" GenerateEntities options:");
            Console.WriteLine("   --ConnectionString | -cs    Required. Connection string to the database server.");
            Console.WriteLine("                               See https://www.connectionstrings.com");
            Console.WriteLine("   --Attribute        | -a     Include the Column attribute if necessary.");
            Console.WriteLine("                               If value is empty, the Apps72 Column attribute is added.");
            Console.WriteLine("                               You can set the full qualified name of attribute to add ");
            Console.WriteLine("                               in addition of Apps72.");
            Console.WriteLine("                               Ex: -a=\"System.ComponentModel.DataAnnotations.Schema.Column\"");
            Console.WriteLine("   --ClassFormat      | -cf    Format of class: NameOnly or SchemaAndName.");
            Console.WriteLine("   --CodeAnalysis     | -ca    Exception codes to add in top of file to avoid Code Analysis ");
            Console.WriteLine("                               warning. Code separator is ','. Ex: AV1706, AV1507).");
            Console.WriteLine("   --Language         | -l     Target format: CSharp (only this one at this moment).");
            Console.WriteLine("   --Namespace        | -ns    Name of the namespace to generate.");
            Console.WriteLine("   --Output           | -o     File name where class will be written.");
            Console.WriteLine("   --OnlySchema       | -os    Only for the specified schema.");
            Console.WriteLine("   --Provider         | -p     Type of server: SqlServer, Oracle or SqLite.");
            Console.WriteLine();
            Console.WriteLine("     By default, Provider=SqlServer, Output=Entities.cs, Language=CSharp");
            Console.WriteLine("                 Namespace=[Empty], ClassFormat=NameOnly");
            Console.WriteLine();
            Console.WriteLine(" Merge options:");
            Console.WriteLine("   --Source           | -s     Source directory pattern containing all files to merged.");
            Console.WriteLine("                               Default is \"*.sql\" in current directory.");
            Console.WriteLine("   --Output           | -o     File name where all files will be merged.");
            Console.WriteLine("                               If not set, the merged file will be written to the console.");
            Console.WriteLine("   --Separator        | -sp    adds this separator between each merged files.");
            Console.WriteLine("                               Ex: -sp=GO");
            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("  DbCmd GenerateEntities -cs=\"Server=localhost;Database=Scott;\" -p=SqlServer -a ");
            Console.WriteLine("  DbCmd Merge --source=\"C:\\Temp\\*.sql\" --output=allScripts.sql");
        }
    }
}
