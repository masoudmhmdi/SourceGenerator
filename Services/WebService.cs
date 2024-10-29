using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant.Enums;
using SourceGenerator.Constant.Type;
using SourceGenerator.Extensions;
using SourceGenerator.FactoryModule;
using SourceGenerator.FactoryModule.FactoryContracts;
using SourceGenerator.ReaderModule;
using SourceGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace SourceGenerator.Services
{
    public static class WebService
    {
        private static bool IsControllerExist(string path)
        {
            return File.Exists(path);
        }

        private static string GetControllerPath(Config config)
        {
            var controllerPath = Path.Combine(Util.GetSolutionParentPath(),config._controllerPath, $"{config._controllerName}.cs");
            return controllerPath;
        }

        public static void Run(FileReader fileReader)
        {
            var config = fileReader.GetConfig();
            var response = fileReader.GetResponse();
            var nestedResponseTypes = fileReader.ExtractNestedClasses(response);
            var request = fileReader.GetRequest();

            var controllerPath = GetControllerPath(config);
            var isControllerExist = IsControllerExist(controllerPath);
            if (isControllerExist)
            {
                // Step1: Read controller 
                var controller = FileReader.ReadFile(controllerPath).GetRoot() as CompilationUnitSyntax;
                // Step 2: Find the class declaration (controller class)
                var classDeclaration = controller.DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .FirstOrDefault(c => c.Identifier.Text.Contains(config._controllerName));
                // Step 3: Add action method
                var declarationWithNewMethod = FileFactory.AddActionMethod(classDeclaration, new AddActionMethodRequest(config._verb, config._apiName));
                // Step 4: Replace controller with onld version
                var modifiedRoot = controller.ReplaceNode(classDeclaration, declarationWithNewMethod);
                // Step 5 :Write file 
                Util.WriteTo(controllerPath, modifiedRoot.NormalizeWhitespace().ToFullString());
            }
            else
            {
                // Create new controller
                var newController = FileFactory.CreateController(controllerPath, config._controllerName);
                // Step 3: Add action method

                var imports = new[]
                {
                    UsingDirective(ParseName("System")),
                    UsingDirective(ParseName("Microsoft.AspNetCore.Mvc"))
                };

                var namespaceName = Util.GenerateNamespaceByPath(config._controllerPath);

                var withActionMethodController = FileFactory
                    .AddActionMethod(newController, new AddActionMethodRequest(config._verb, config._apiName))
                    .WriteAsNamespace(controllerPath,namespaceName,imports);
            }
                // write request and response
                WriteRequestAndResponse(config, request, response,nestedResponseTypes);

        }


        public static void WriteRequestAndResponse(Config config, ClassDeclarationSyntax request, ClassDeclarationSyntax response , IEnumerable<ClassDeclarationSyntax> nestedResponseTypes)
        {
            var imports = new UsingDirectiveSyntax[] {};

            
            var requestPath = Path.Combine(config._requestPath, config._apiName, $"{config._apiName}Request.cs");
            var requestNamespaceName = Util.GenerateNamespaceByPath(config._requestPath);
            var finalRequest = request.WithIdentifier(Identifier($"{config._apiName}Request"));


            string responsePath;
            if(config._responsePath == config._CQRSPath)
            {
                responsePath = Path.Combine(config._responsePath, config._apiName, $"{config._apiName}Result.cs");
            }
            else
            {
                responsePath = Path.Combine(config._responsePath, $"{config._apiName}Result.cs");
            }

            var responseNamespaceName = Util.GenerateNamespaceByPath(config._responsePath);
            var finalResponse = response.WithIdentifier(Identifier($"{config._apiName}Result"));


            var responseResult = new List<ClassDeclarationSyntax>()
            {
                finalResponse,
            };

            responseResult.AddRange(nestedResponseTypes);

            responseResult.WriteAsNamespace(responsePath, responseNamespaceName, imports);

            finalRequest.WriteAsNamespace(requestPath,requestNamespaceName,imports);
            //finalResponse.WriteAsNamespace(responsePath,responseNamespaceName,imports);
        }



    }
}
