using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant;
using SourceGenerator.Constant.Type;
using SourceGenerator.Utils;

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


        public Config GetConfig()
        {
            var ClassDeclaration = _templateRoot.DescendantNodes()
               .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(c => c.Identifier.Text == Template.Config);
            if (ClassDeclaration == null) throw new Exception("config type not found");

            var config = new Config
            {
                _apiName = Util.GetClassMemberDefaultValue(ClassDeclaration, "_apiName"),
                _controllerName = Util.GetClassMemberDefaultValue(ClassDeclaration, "_controllerName"),
                _controllerPath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_controllerPath"),
                _requestPath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_requestPath"),
                _responsePath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_responsePath"),
                _CQRSPath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_CQRSPath"),
                _verb = Util.GetClassMemberDefaultValue(ClassDeclaration, "_verb"),
            };

            return config;
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


        public static SyntaxTree ReadFile(string path)
        {
            var solutionPath = Util.GetSolutionParentPath();
            var finalPath = Path.Combine(solutionPath, path);
            var file = File.ReadAllText(finalPath);
            return CSharpSyntaxTree.ParseText(file);

        }
    }
}
