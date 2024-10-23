using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant;

namespace SourceGenerator.ReaderModule
{
    public class FileReader
    {
        private readonly CompilationUnitSyntax _templateRoot;

        public FileReader()
        {
            _templateRoot = ReadInitialTemplate().GetCompilationUnitRoot();
        }


        private static SyntaxTree ReadInitialTemplate()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory, $"InitialTemplate.cs");

            try
            {
                var programText = File.ReadAllText(filePath);
            // Parse the code into a syntax tree
            return CSharpSyntaxTree.ParseText(programText);
            }
            catch
            {
                throw new DirectoryNotFoundException("initial file is not found");
            }

        }


        public ClassDeclarationSyntax? GetConfig()
        {
            return _templateRoot.DescendantNodes()
               .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.Text == Template.Config);
            
        }

        public ClassDeclarationSyntax? GetRequest()
        {
            return _templateRoot.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.Text == Template.Request);
        }

        public ClassDeclarationSyntax? GetResponse()
        {
            return _templateRoot.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.Text == Template.Response);
        }
    }
}
