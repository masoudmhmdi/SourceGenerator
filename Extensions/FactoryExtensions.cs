using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Utils;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace SourceGenerator.Extensions
{
    public static class FactoryExtensions
    {
        public static TypeDeclarationSyntax WriteAsNamespace(this TypeDeclarationSyntax typeDeclrationSyntax, string path, string namespaceName, IEnumerable<UsingDirectiveSyntax> usings)
        {
            // create a new nampspace with it's imports
            var namespaceDeclaration = NamespaceDeclaration(ParseName(namespaceName));
                    
            namespaceDeclaration = namespaceDeclaration.AddMembers(typeDeclrationSyntax);
            // Create the compilation unit (the root node of the syntax tree)
            var compilationUnit = CompilationUnit()
                .AddUsings(usings.ToArray())
                .AddMembers(namespaceDeclaration)
                .NormalizeWhitespace();
            // Write it
            Util.WriteTo(path, compilationUnit.ToFullString());

            return typeDeclrationSyntax;
        }


        public static IEnumerable<TypeDeclarationSyntax> WriteAsNamespace(this IEnumerable<TypeDeclarationSyntax> typeDeclrationSyntaxList, string path, string namespaceName, IEnumerable<UsingDirectiveSyntax> usings)
        {
            // create a new nampspace with it's imports
            var namespaceDeclaration = NamespaceDeclaration(ParseName(namespaceName))
                .AddMembers(typeDeclrationSyntaxList.ToArray());


            
            // Create the compilation unit (the root node of the syntax tree)
            var compilationUnit = CompilationUnit()
                .AddUsings(usings.ToArray())
                .AddMembers(namespaceDeclaration)
                .NormalizeWhitespace();
            // Write it
            Util.WriteTo(path, compilationUnit.ToFullString());

            return typeDeclrationSyntaxList;
        }
    }
}
