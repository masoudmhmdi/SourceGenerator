using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.FactoryModule;
using SourceGenerator.ReaderModule;
using SourceGenerator.Utils;

namespace SourceGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileFactory.GenerateInitialTemplateFile();
            var tree = new FileReader();

            var config = tree.GetConfig();
            var request = tree.GetRequest();
            var response = tree.GetResponse();

            var controller = FileFactory.AddController("", "MamadController").ToFullString();

            Util.WriteTo("", controller,"mmdController.cs");



            var solutionPath = Util.FindSolutionPath(Directory.GetCurrentDirectory());

        }



    }
}
