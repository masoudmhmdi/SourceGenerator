using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.FactoryModule;
using SourceGenerator.ReaderModule;

namespace SourceGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileFactory.GenerateInitialTemplateFile();
            var tree = new Reader();

            var x = tree.GetConfig();



            //var inputClass = root.DescendantNodes()
            //    .OfType<ClassDeclarationSyntax>()
            //    .FirstOrDefault(c => c.Identifier.Text == "Controller");
            //var x = tree.ToString();
            //if (inputClass is null) return;


            //var fields = inputClass.Members.OfType<FieldDeclarationSyntax>();
            //var namespaceDeclaration = inputClass.FirstAncestorOrSelf<NamespaceDeclarationSyntax>().Name.ToString();

            //foreach (var field in fields)
            //{
            //    var variables = field.Declaration.Variables;
            //    foreach (var variable in variables)
            //    {
            //        // Field name
            //        var fieldName = variable.Identifier.Text;

            //        // Check if the field has an initializer (i.e., a value assignment)
            //        if (variable.Initializer != null)
            //        {

            //            // Get the value assigned to the field
            //            var fieldValue = variable.Initializer.Value;
            //            var variableKind = variable.Kind();



            //            Console.WriteLine($"Field: {fieldName}, Value: {fieldValue}");
            //        }
            //        else
            //        {
            //            Console.WriteLine($"Field: {fieldName}, No Initial Value");
            //        }
            //    }
            //}

            //// Create a new method
            //MethodDeclarationSyntax newMethod = SyntaxFactory.MethodDeclaration(
            //        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
            //        "NewMethod")
            //    .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword)) // public method
            //    .WithBody(SyntaxFactory.Block(
            //        SyntaxFactory.ParseStatement(@"Console.WriteLine(""Hello from NewMethod"");")
            //    ));
            //// Add the new method to the class
            //ClassDeclarationSyntax modifiedClass = inputClass.AddMembers(newMethod);

            //// Replace the old class with the modified class in the syntax tree
            //root = root.ReplaceNode(inputClass, modifiedClass);

            //Console.WriteLine(root.NormalizeWhitespace().ToFullString());
        }


        public static class Tools
        {
            public static string FindSolutionPath(string directory)
            {
                while (!string.IsNullOrEmpty(directory))
                {
                    // Check if a .sln file exists in the current directory
                    string[] solutionFiles = Directory.GetFiles(directory, "*.sln");
                    if (solutionFiles.Length > 0)
                    {
                        return directory;
                    }

                    // Move up to the parent directory
                    directory = Directory.GetParent(directory)?.FullName;
                }

                return null; // No solution found
            }
        }

    }
}
