using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "GeneratedTemplate.cs");
            // Write the code to a file
            File.WriteAllText(destinationPath, formattedCode.ToFullString());
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

public class TestTemplate
{

}