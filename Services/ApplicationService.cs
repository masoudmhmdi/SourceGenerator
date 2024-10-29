using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant.Enums;
using SourceGenerator.Constant.Type;
using SourceGenerator.Extensions;
using SourceGenerator.FactoryModule;
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
    public static class ApplicationService
    {
        public static void Run(FileReader fileReader)
        {
            var config = fileReader.GetConfig();
            var request = fileReader.GetRequest();
            var response = fileReader.GetResponse();



            CheckCQRSPathExist(config._CQRSPath);


            var path = Path.Combine(Util.GetSolutionParentPath(), config._CQRSPath);



            // Create a namespace and add the class to it
            var namespaceDeclaration = Util.GenerateNamespaceByPath(config._CQRSPath);
            var imports = new List<UsingDirectiveSyntax>()
            {
                    UsingDirective(IdentifierName("System")),
                    UsingDirective(IdentifierName("System.Threading")),
                    UsingDirective(IdentifierName("System.Threading.Tasks"))
            };

            switch (config._verb)
            {
                case HttpVerb.GET:
                    CreateQuery(config, response)
                        .WriteAsNamespace(Path.Combine(path, $"{config._apiName}Query.cs"), namespaceDeclaration, imports);
                    CreateQueryHandler(config)
                        .WriteAsNamespace(Path.Combine(path, $"{config._apiName}QueryHandler.cs"), namespaceDeclaration, imports);
                    break;
                case HttpVerb.POST:
                case HttpVerb.PUT:
                case HttpVerb.DELETE:
                    CreateCommand(config, request)
                        .WriteAsNamespace(Path.Combine(path,$"{config._apiName}Command.cs"),namespaceDeclaration,imports);
                    CreateCommandHandler(config)
                        .WriteAsNamespace(Path.Combine(path, $"{config._apiName}CommandHandler.cs"), namespaceDeclaration, imports);
                    break;
            }
        }


        public static void CheckCQRSPathExist(string path)
        {
            var directoryExist = Directory.Exists(Path.Combine(Util.GetSolutionParentPath(), path));
            if(!directoryExist)
            {
                throw new Exception("CQRS directory not found");
            }
        }


        public static RecordDeclarationSyntax CreateCommand(Config config, ClassDeclarationSyntax request)
        {
            var parameters = Util.ExtractProperties(request);
            var queryRecord = FileFactory.CreateRecord($"{config._apiName}Command", parameters, "ICommand", $"{config._apiName}Result");
            return queryRecord;
        }
        
        public static ClassDeclarationSyntax CreateCommandHandler(Config config)
        {
            // Define the class name
            var className = $"{config._apiName}CommandHandler";

            // Define the implemented interface
            var interfaceName = IdentifierName("ICommandHandler");
            var typeArgumentList = TypeArgumentList(
                SeparatedList<TypeSyntax>(
                    new SyntaxNodeOrToken[]
                    {
                    IdentifierName($"{config._apiName}Command"),
                    Token(SyntaxKind.CommaToken),
                    IdentifierName($"{config._apiName}Result")
                    }
                ));
            var baseInterface = GenericName(interfaceName.Identifier).WithTypeArgumentList(typeArgumentList);

            // Create the class declaration with an interface
            var classDeclaration = ClassDeclaration(className)
                .AddModifiers(Token(SyntaxKind.InternalKeyword))
                .AddBaseListTypes(SimpleBaseType(baseInterface));

            // Create the constructor
            var constructor = ConstructorDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithBody(Block());

            // Create the Handle method
            var handleMethod = MethodDeclaration(
                    GenericName("Task").WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                GenericName("Result")
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName($"{config._apiName}Result")
                                            )
                                        )
                                    )
                            )
                        )
                    ),
                    Identifier("Handle")
                )
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
                .AddParameterListParameters(
                    Parameter(Identifier("request")).WithType(IdentifierName($"{config._apiName}Command")),
                    Parameter(Identifier("cancellationToken")).WithType(IdentifierName("CancellationToken"))
                )
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ThrowStatement(
                                ObjectCreationExpression(IdentifierName("NotImplementedException"))
                                    .WithArgumentList(ArgumentList())
                            )
                        )
                    )
                )
                .WithExplicitInterfaceSpecifier(
                    ExplicitInterfaceSpecifier(baseInterface)
                );

            // Combine everything into the class
            classDeclaration = classDeclaration
                .AddMembers(constructor, handleMethod);

            return classDeclaration;
        }
        public static RecordDeclarationSyntax CreateQuery(Config config,ClassDeclarationSyntax response)
        {
            var parameters = Util.ExtractProperties(response);
            var queryRecord = FileFactory.CreateRecord($"{config._apiName}Query", parameters, "IQuery", $"{config._apiName}Result");
            return queryRecord;

        }
        public static ClassDeclarationSyntax CreateQueryHandler(Config config)
        {
            // Create the class declaration
            var classDeclaration = ClassDeclaration($"{config._apiName}QueryHandler")
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword))
                .AddBaseListTypes(
                    SimpleBaseType(
                        ParseTypeName($"IQueryHandler<{config._apiName}Query, {config._apiName}Result>")));

            // Constructor
            var constructorDeclaration = ConstructorDeclaration($"{config._apiName}QueryHandler")
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .WithBody(Block());

            // Async Handle Method Implementation
            var methodDeclaration = MethodDeclaration(
                    GenericName("Task")
                        .AddTypeArgumentListArguments(
                            GenericName("Result")
                                .AddTypeArgumentListArguments(
                                    IdentifierName($"{config._apiName}Result"))),
                    Identifier("Handle"))
                .AddModifiers(Token(SyntaxKind.AsyncKeyword))
                .WithExplicitInterfaceSpecifier(
                    ExplicitInterfaceSpecifier(
                        ParseName($"IRequestHandler<{config._apiName}Query, Result<{config._apiName}Result>>")))
                .WithParameterList(
                    ParameterList(SeparatedList(new[]
                    {
                    Parameter(Identifier("request"))
                        .WithType(IdentifierName($"{config._apiName}Query")),
                    Parameter(Identifier("cancellationToken"))
                        .WithType(IdentifierName("CancellationToken"))
                    })))
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ThrowStatement(
                                ObjectCreationExpression(
                                    IdentifierName("NotImplementedException"))
                                .WithArgumentList(ArgumentList())))));

            // Add members to the class
            classDeclaration = classDeclaration.AddMembers(constructorDeclaration, methodDeclaration);

            return classDeclaration;
            
        }


    }
}


//internal class CreateEntertainmentQuestionnaireCommandHandler : ICommandHandler<CreateEntertainmentQuestionnaireCommand, CreateEntertainmentQuestionnaireResult>
//{
//    public CreateEntertainmentQuestionnaireCommandHandler()
//    {
//    }

//    public async Task<Result<CreateEntertainmentQuestionnaireResult>> Handle(CreateEntertainmentQuestionnaireCommand request, CancellationToken cancellationToken)
//    {
//        throw new NotImplementedException();
//    }
//}