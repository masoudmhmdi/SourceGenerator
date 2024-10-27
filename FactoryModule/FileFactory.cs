using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceGenerator.Constant.Enums;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SourceGenerator.FactoryModule.FactoryContracts;
using SourceGenerator.Utils;

namespace SourceGenerator.FactoryModule
{
    public static class FileFactory
    {

        public static void GenerateInitialTemplateFile()
        {
            // Create the static Config class
            ClassDeclarationSyntax configClass = SyntaxFactory.ClassDeclaration("Config")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .AddMembers(
                    CreateField("_controllerPath", "/"),
                    CreateField("_controllerName", "/"),
                    CreateField("_apiName", "/"),
                    CreateField("_verb", "/"),
                    CreateField("_CQRSPath", "/")
                );

            // Create the Request class
            ClassDeclarationSyntax requestClass = SyntaxFactory.ClassDeclaration("Request")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create the Response class
            ClassDeclarationSyntax responseClass = SyntaxFactory.ClassDeclaration("Response")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

            // Create the namespace
            NamespaceDeclarationSyntax namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Template"))
                .AddMembers(configClass, requestClass, responseClass);

            // Add using directives
            CompilationUnitSyntax compilationUnit = SyntaxFactory.CompilationUnit()
                .AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")))
                .AddMembers(namespaceDeclaration);

            // Normalize and format the code
            SyntaxNode formattedCode = compilationUnit.NormalizeWhitespace();

            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "InitialTemplate.cs");
            // Write the code to a file
            File.WriteAllText(destinationPath, formattedCode.ToFullString());
        }


        public static ClassDeclarationSyntax AddActionMethod(ClassDeclarationSyntax classDeclaration, AddActionMethodRequest methodRequest)
        {
            // create attribute

              var httpGetAttribute = Attribute(
                  ParseName($"Http{methodRequest.Verb}"),
                  AttributeArgumentList(
                      SingletonSeparatedList(
                          AttributeArgument(
                              LiteralExpression(
                                  SyntaxKind.StringLiteralExpression,
                                  Literal(methodRequest.Name)
                              )
                          )
                      )
                  )
                );

            // Create a method declaration
            var methodDeclaration = MethodDeclaration(
                    PredefinedType(Token(SyntaxKind.StringKeyword)),
                    Identifier(methodRequest.Name)
                )
                .WithModifiers(
                    TokenList(Token(SyntaxKind.PublicKeyword))
                )
                .WithAttributeLists(
                    SingletonList(
                        AttributeList(
                            SingletonSeparatedList(httpGetAttribute)
                        )
                    )
                )
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ReturnStatement(
                                LiteralExpression(
                                    SyntaxKind.StringLiteralExpression,
                                    Literal("Hello World!")
                                )
                            )
                        )
                    )
                );

            // Add the method to the class
            classDeclaration = classDeclaration.AddMembers(methodDeclaration);

            return classDeclaration;
        }


        public static ClassDeclarationSyntax AddController(string destinationPath,string controllerName)
        {
            // Create a namespace declaration
            var namespaceDeclaration = NamespaceDeclaration(ParseName("YourApp.Controllers"))
                .WithUsings(
                    List(new[]
                    {
                    UsingDirective(ParseName("System")),
                    UsingDirective(ParseName("Microsoft.AspNetCore.Mvc"))
                    })
                );

            // Create a class declaration
            var classDeclaration = ClassDeclaration(controllerName)
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)
                    )
                )
                .WithBaseList(
                    BaseList(
                        SingletonSeparatedList<BaseTypeSyntax>(
                            SimpleBaseType(ParseTypeName("ControllerBase"))
                        )
                    )
                );

            //// Add the class to the namespace
            //namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclarationWithMethod);

            //// Create the compilation unit (the root node of the syntax tree)
            //var compilationUnit = CompilationUnit()
            //    .AddMembers(namespaceDeclaration)
            //    .NormalizeWhitespace();

            return classDeclaration;
        }

        private static FieldDeclarationSyntax CreateField(string fieldName, string value)
        {
            return SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                            SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))
                        .AddVariables(SyntaxFactory.VariableDeclarator(fieldName)
                            .WithInitializer(
                                SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value))))))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
        }
    }
}

