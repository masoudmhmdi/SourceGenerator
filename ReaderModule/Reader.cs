using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator.ReaderModule
{
    public class Reader
    {
        private readonly CompilationUnitSyntax _templateRoot;

        public Reader()
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
                .FirstOrDefault(c => c.Identifier.Text == "Config");
            
        }
        public void GetRequest(){}
        public void GetResponse() { }
    }
}
