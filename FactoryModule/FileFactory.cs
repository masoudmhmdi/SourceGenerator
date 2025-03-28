﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                    CreateField("_controllerPath", ""),
                    CreateField("_controllerName", ""),
                    CreateField("_apiName", ""),
                    CreateField("_requestPath", ""),
                    CreateField("_responsePath", ""),
                    CreateField("_verb", ""),
                    CreateField("_CQRSPath", "")
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


            // Define the return type Task<ActionResult<ApiResponse<CreateEntertainmentQuestionnaireResult>>>
               var returnType = GenericName("Task")
                   .WithTypeArgumentList(
                       TypeArgumentList(
                           SingletonSeparatedList<TypeSyntax>(
                               GenericName("ActionResult")
                                   .WithTypeArgumentList(
                                       TypeArgumentList(
                                           SingletonSeparatedList<TypeSyntax>(
                                               GenericName("ApiResponse")
                                                   .WithTypeArgumentList(
                                                       TypeArgumentList(
                                                           SingletonSeparatedList<TypeSyntax>(
                                                               IdentifierName($"{methodRequest.Name}Result")
                                                           )
                                                       )
                                                   )
                                           )
                                       )
                                   )
                           )
                       )
                   );

            // Create the throw statement for NotImplementedException
            var throwStatement = ThrowStatement(
                ObjectCreationExpression(IdentifierName("NotImplementedException"))
                    .WithArgumentList(ArgumentList())
            );


            // Create a method declaration
            var methodDeclaration = MethodDeclaration(
                           returnType,
                           Identifier(methodRequest.Name)
                       )
                       .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword))
                       .WithAttributeLists(
                           SingletonList(
                               AttributeList(
                                   SingletonSeparatedList(httpGetAttribute)
                               )
                           )
                       )
                       .WithBody(Block(throwStatement));



            // Add the method to the class
            classDeclaration = classDeclaration.AddMembers(methodDeclaration);

            return classDeclaration;
        }
        public static ClassDeclarationSyntax CreateController(string destinationPath,string controllerName)
        {

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

        public static RecordDeclarationSyntax CreateRecord(
            string recordName,
            IEnumerable<ParameterSyntax> parameters,
            string baseInterface,
            string resultType)
        {

            // Build the list of parameters dynamically
            //var parameterSyntaxList = new List<ParameterSyntax>();
            //foreach (var (type, name) in parameters)
            //{
            //    Console.WriteLine(type);

            //    // Check if the type is a predefined type (e.g., int, string, bool)
            //    TypeSyntax parameterType;
            //    if (type == SyntaxKind.IntKeyword || type == SyntaxKind.StringKeyword || type == SyntaxKind.BoolKeyword)
            //    {
            //        // Use PredefinedType for known primitive types
            //        parameterType = PredefinedType(Token(type));
            //    }
            //    else
            //    {
            //        // Otherwise, treat as a custom type with IdentifierName
            //        parameterType = IdentifierName(type.ToString());
            //    }

            //    var parameter = Parameter(Identifier(name))
            //        .WithType(parameterType);

            //    parameterSyntaxList.Add(parameter);
            //}

            // Create the base type for the interface implementation
            var baseType = SimpleBaseType(
                GenericName(Identifier(baseInterface))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(resultType)
                            )
                        )
                    )
            );

            // Assemble the full record declaration
            var recordDeclaration = RecordDeclaration(Token(SyntaxKind.RecordKeyword), Identifier(recordName))
                .WithModifiers(
                    TokenList(new[]
                    {
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.SealedKeyword)
                    })
                )
                .WithParameterList(
                    ParameterList(SeparatedList(parameters))
                )
                .WithBaseList(
                    BaseList(SingletonSeparatedList<BaseTypeSyntax>(baseType))
                )
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            return recordDeclaration;

        }


    }
}



