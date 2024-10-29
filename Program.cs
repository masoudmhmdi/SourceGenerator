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
            //if(args.Length != 1)
            //{
            //    return;
            //}


            //if (args[0] == Commands.Init)
            //{
            //    FileFactory.GenerateInitialTemplateFile();
            //}


            if ("Run" == Commands.Run)
            {
                var fileReader = new FileReader();
                WebService.Run(fileReader);
                ApplicationService.Run(fileReader);

            }

            //if (args[0] == "Test")
            //{
            //    var solutionPath = Util.GetSolutionParentPath();
            //    var currentDirectory = Directory.GetCurrentDirectory();
            //    var parentSolution = Directory.GetParent(solutionPath)?.FullName;

            //    Console.WriteLine(@$"
            //                           SlutionPath:{solutionPath}
            //                           currentDirectory:{currentDirectory}
            //                           parentSolution:{parentSolution}
    
            //                        ");

            //}


        }



    }
}
