using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
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
        
        
        private static string? GetSolutionName(string? directory)
        {
            while (!string.IsNullOrEmpty(directory))
            {
                // Check if a .sln file exists in the current directory
                string[] solutionFiles = Directory.GetFiles(directory, "*.sln");
                if (solutionFiles.Length > 0)
                {
                    return Path.GetFileNameWithoutExtension(solutionFiles[0]);
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
            var solution = GetSolutionName(Directory.GetCurrentDirectory());
            var responseNamespaceName = String.Join(".", path.Split(new char[] { '/', '\\' }));
            return solution + "." + responseNamespaceName;
        }


        public static IEnumerable<ParameterSyntax> ExtractProperties(ClassDeclarationSyntax classDeclarationSyntax)
        {


            // Get all properties in the class
            var properties = classDeclarationSyntax.Members
                .OfType<PropertyDeclarationSyntax>();

            // Create parameters for the record from the properties
            var parameters = properties.Select(property =>
                Parameter(Identifier(property.Identifier.Text))
                    .WithType(property.Type)
            ).ToArray();

            return parameters;



            //var x = new List<(SyntaxKind, string)>();

            //// Iterate over all members of the class
            //foreach (var member in classDeclarationSyntax.Members)
            //{
            //    // Check if the member is a property
            //    if (member is PropertyDeclarationSyntax property)
            //    {
            //        // Get the SyntaxKind of the property type
            //        var typeKind = property.Type.Kind();

            //        // Get the name of the property
            //        var propertyName = property.Identifier.Text;

            //        // Add the type and name as a tuple to the list
            //        properties.Add((typeKind, propertyName));
            //    }
            //}

            //return x;
        }
    }

}
