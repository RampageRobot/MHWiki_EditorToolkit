namespace MHWikiToolkit
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && HelpRequired(args[0]))
            {
                DisplayHelp();
            }
            else
            {
                switch (args[0])
                {
                    case "DamageTable":
                        MediawikiTranslator.Generators.DamageTable.ParseZip(args[1], args[2]);
                        Console.WriteLine("Done!");
                        break;
                }
            }
        }

        private static bool HelpRequired(string param)
        {
            return param == "-h" || param == "--help" || param == "/?";
        }

        private static void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("---MHWikiToolkit---");
            Console.WriteLine("Arguments:");
            Console.WriteLine("1: Data Type. Supported type(s): DamageTable");
            Console.WriteLine("2: Source Path. This is expected to be a path on your local machine that leads to a .zip file containing the JSON data you wish to format.");
            Console.WriteLine("3: Destination Path. This is expected to be a directory on your local machine to save the text files containing the formatted MediaWiki code to. If the directory does not exist, it'll be created.");
            Console.WriteLine("-------------------");
        }
    }
}
