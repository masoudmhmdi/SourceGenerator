using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant;
using SourceGenerator.Constant.Enums;
using SourceGenerator.FactoryModule;
using SourceGenerator.FactoryModule.FactoryContracts;
using SourceGenerator.ReaderModule;
using SourceGenerator.Utils;

namespace SourceGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length != 1)
            {
                return;
            }


            if (args[0] == Commands.Init)
            {
                FileFactory.GenerateInitialTemplateFile();
            }


            if (args[0] == Commands.Run)
            {

                var tree = new FileReader();

                var config = tree.GetConfig();
                var request = tree.GetRequest();
                var response = tree.GetResponse();


                var controller = FileFactory.AddController("", "MamadController");
                var classDeclarationWithMethod = FileFactory.AddActionMethod(controller, new AddActionMethodRequest(HttpVerb.POST, "getMmd"));
                Util.WriteTo("", classDeclarationWithMethod.NormalizeWhitespace().ToFullString(), "mmdController.cs");


                var solutionPath = Util.FindSolutionPath(Directory.GetCurrentDirectory());
                FileReader.ReadFile("test\\TestController.cs");
            }




        }



    }
}
