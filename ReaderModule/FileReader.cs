using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant;
using SourceGenerator.Constant.Enums;
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

        public List<ClassDeclarationSyntax> ExtractNestedClasses(ClassDeclarationSyntax parentClass)
        {
            var nestedClasses = new List<ClassDeclarationSyntax>();

            // Traverse the members of the parent class and find nested classes
            foreach (var member in parentClass.Members)
            {
                if (member is ClassDeclarationSyntax nestedClass)
                {
                    nestedClasses.Add(nestedClass);
                    nestedClasses.AddRange(this.ExtractNestedClasses(nestedClass));

                }


                if(member is PropertyDeclarationSyntax property)
                {
                    if (property.Type is GenericNameSyntax genericName &&
                        //genericName.Identifier.Text == "List" &&
                        genericName.TypeArgumentList.Arguments.Count == 1)
                    {
                        // Get the type of the generic argument
                        var genericTypeName = genericName.TypeArgumentList.Arguments[0].ToString();
                        var genericType = _templateRoot.DescendantNodes()
                            .OfType<ClassDeclarationSyntax>()
                            .FirstOrDefault(c => c.Identifier.Text == genericTypeName.Trim());

                        if(genericType == null)
                        {
                            throw new Exception("nested class declration not found!");
                        }
                        nestedClasses.Add(genericType);
                        nestedClasses.AddRange(this.ExtractNestedClasses(genericType));
                        

                    }
                    
                }

                //if (property.Type is GenericNameSyntax genericName &&
                //        genericName.Identifier.Text == "List" &&
                //        genericName.TypeArgumentList.Arguments.Count == 1)
                //{
                //    // Get the type of the generic argument
                //    var genericType = genericName.TypeArgumentList.Arguments[0].ToString();
                //    Console.WriteLine($"Property: {property.Identifier.Text}, Type: List<{genericType}>");
                //}
            }

            return nestedClasses;
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

            var verb = Util.GetClassMemberDefaultValue(ClassDeclaration, "_verb");

            var config = new Config
            {
                _apiName = Util.GetClassMemberDefaultValue(ClassDeclaration, "_apiName"),
                _controllerName = Util.GetClassMemberDefaultValue(ClassDeclaration, "_controllerName"),
                _controllerPath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_controllerPath"),
                _requestPath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_requestPath"),
                _responsePath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_responsePath"),
                _CQRSPath = Util.GetClassMemberDefaultValue(ClassDeclaration, "_CQRSPath"),
                _verb = verb switch
                {
                    "GET" => HttpVerb.GET,
                    "POST" => HttpVerb.POST,
                    "PUT" => HttpVerb.PUT,
                    "DELETE" => HttpVerb.DELETE,
                    "PATCH" => HttpVerb.PATCH,
                    _=> HttpVerb.GET
                }
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
