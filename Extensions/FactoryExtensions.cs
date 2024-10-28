using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace SourceGenerator.Extensions
{
    public static class FactoryExtensions
    {
        public static ClassDeclarationSyntax WriteAsNamespace(this ClassDeclarationSyntax classDeclarationSyntax, string path, string namespaceName, IEnumerable<UsingDirectiveSyntax> usings)
        {
            var namespaceDeclaration = NamespaceDeclaration(ParseName(namespaceName))
                    .WithUsings(
                        List(usings)
                    );
            namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclarationSyntax);
            // Create the compilation unit (the root node of the syntax tree)
            var compilationUnit = CompilationUnit()
                .AddMembers(namespaceDeclaration)
                .NormalizeWhitespace();
            // Write it
            Util.WriteTo(path, compilationUnit.ToFullString());

            return classDeclarationSyntax;
        }
    }
}
