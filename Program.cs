using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant;
using SourceGenerator.Constant.Enums;
using SourceGenerator.FactoryModule;
using SourceGenerator.FactoryModule.FactoryContracts;
using SourceGenerator.ReaderModule;
using SourceGenerator.Services;
using SourceGenerator.Utils;

namespace SourceGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                return;
            }


            if (args[0] == Commands.Init)
            {
                FileFactory.GenerateInitialTemplateFile();
            }


            if (args[0] == Commands.Run)
            {
                var fileReader = new FileReader();
                WebService.Run(fileReader);
                ApplicationService.Run(fileReader);

            }

            if (args[0] == Commands.Test)
            {
                var root = Util.GetSolutionParentPath();
                var currentDirectory = Directory.GetCurrentDirectory();

                Console.WriteLine(@$"
                                       currentDirectory:{currentDirectory}
                                       rootPath:{root}
                                    ");
            }


        }



    }
}
