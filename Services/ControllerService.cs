using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant.Enums;
using SourceGenerator.Constant.Type;
using SourceGenerator.FactoryModule;
using SourceGenerator.FactoryModule.FactoryContracts;
using SourceGenerator.ReaderModule;
using SourceGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator.Services
{
    public static class ControllerService
    {
        public static void AddController(Config config)
        {

            var controllerPath = Path.Combine(config._controllerPath,$"{ config._controllerName}.cs");

            var controller = FileReader.ReadFile(controllerPath).GetRoot() as CompilationUnitSyntax;
            Console.WriteLine(controller.ToFullString());

            // Step 2: Find the class declaration (controller class)
            var classDeclaration = controller.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.Text.Contains(config._controllerName));

            var declarationWithNewMethod = FileFactory.AddActionMethod(classDeclaration, new AddActionMethodRequest(HttpVerb.GET, config._apiName));
            var modifiedRoot = controller.ReplaceNode(classDeclaration, declarationWithNewMethod);
            Util.WriteTo(controllerPath, modifiedRoot.NormalizeWhitespace().ToFullString());


        }




    }
}
