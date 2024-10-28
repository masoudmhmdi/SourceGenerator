using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceGenerator.Constant.Type;

namespace SourceGenerator.Utils
{
    public static class Util
    {
        private static string? FindSolutionDirectoryPath(string? directory)
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


        public static string GetClassMemberDefaultValue(ClassDeclarationSyntax classDeclaration, string memberName)
        {
            string value = string.Empty;
            foreach (var member in classDeclaration.Members)
            {
                // Check if the member is a field declaration (i.e., "int Number = 42;")
                if (member is FieldDeclarationSyntax fieldDeclaration)
                {
                    foreach (var variable in fieldDeclaration.Declaration.Variables)
                    {
                        if (variable.Identifier.Text == memberName)
                        {
                            // Check if the field has an initializer
                            if (variable.Initializer != null)
                            {
                                var initializer =(LiteralExpressionSyntax)variable.Initializer.Value;
                                value = initializer.Token.ValueText;
                            }
                        }
                    }
                }
                // Check if the member is a property declaration (i.e., "int Number { get; set; } = 42;")
                //if (member is PropertyDeclarationSyntax propertyDeclaration)
                //{
                //    if (propertyDeclaration.Identifier.Text == memberName)
                //    {
                //        // Check if the property has an initializer
                //        if (propertyDeclaration.Initializer != null)
                //        {
                //            Console.WriteLine($"Property '{memberName}' has a default value: {propertyDeclaration.Initializer.Value}");
                //        }
                //        else
                //        {
                //            Console.WriteLine($"Property '{memberName}' has no default value.");
                //        }
                //    }
                //}
            }

            return value;
        }


        public static void WriteTo(string destinationPath ,string value)
        {
            var solutionPath = GetSolutionParentPath();
            var finalPath = Path.Combine(solutionPath, destinationPath);
            File.WriteAllText(finalPath, value);
        }

        public static string GetSolutionParentPath()
        {
            var solutionDirectory = FindSolutionDirectoryPath(Directory.GetCurrentDirectory());
            if (solutionDirectory is null)
            {
                throw new Exception("Solution not found");
            }

            var rootPath = Directory.GetParent(solutionDirectory).FullName;
            var isDirectoryExist = Directory.Exists(rootPath);
            if (!isDirectoryExist)
            {
                throw new Exception("Root Directory not found");
            }
            return rootPath;
        }


        public static string GenerateNamespaceByPath(string path)
        {
            var responseNamespaceName = String.Join(".", path.Split(new char[] { '/', '\\' }));
            return responseNamespaceName;
        }
    }

}
