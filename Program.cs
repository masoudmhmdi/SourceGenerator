using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Constant;
using SourceGenerator.Constant.Enums;
using SourceGenerator.Constant.Type;
using SourceGenerator.FactoryModule;
using SourceGenerator.FactoryModule.FactoryContracts;
using SourceGenerator.ReaderModule;
using SourceGenerator.Services;
using SourceGenerator.Utils;

namespace SourceGenerator
{
    internal class Program
    {
        static void Main(string[] args)

        {

            if (args.Length != 1)
            {
                var fileReader = new FileReader();
                var config = fileReader.GetConfig();

                var IsCQRSPathExist = Directory.Exists(Path.Combine(Util.GetSolutionParentPath(), config._CQRSPath));
                var IsRequestPathExist = Directory.Exists(Path.Combine(Util.GetSolutionParentPath(), config._requestPath));

                if (IsCQRSPathExist && IsRequestPathExist)
                {
                    var CQRSPathDirectorty = Path.Combine(Util.GetSolutionParentPath(), config._CQRSPath, config._apiName);
                    var requestPathDirectory = Path.Combine(Util.GetSolutionParentPath(), config._requestPath, config._apiName);
                    Directory.CreateDirectory(CQRSPathDirectorty);
                    Directory.CreateDirectory(requestPathDirectory);
                }
                else
                {
                    throw new Exception("CQRS or Request Direcotry not found");
                }

                ApplicationService.Run(fileReader);
                WebService.Run(fileReader);
                Console.WriteLine("""Wrong command: Enter "dontnet sourceGenerator Help" """);
                return;
            }


            if (args[0] == Commands.Init)
            {
                FileFactory.GenerateInitialTemplateFile();
            }


            if (args[0] == Commands.Run)
            {
                var fileReader = new FileReader();
                var config = fileReader.GetConfig();

                var IsCQRSPathExist = Directory.Exists(Path.Combine(Util.GetSolutionParentPath(), config._CQRSPath));
                var IsRequestPathExist = Directory.Exists(Path.Combine(Util.GetSolutionParentPath(), config._requestPath));

                if (IsCQRSPathExist && IsRequestPathExist)
                {
                    var CQRSPathDirectorty = Path.Combine(Util.GetSolutionParentPath(), config._CQRSPath, config._apiName);
                    var requestPathDirectory = Path.Combine(Util.GetSolutionParentPath(), config._requestPath, config._apiName);
                    Directory.CreateDirectory(CQRSPathDirectorty);
                    Directory.CreateDirectory(requestPathDirectory);
                }
                else
                {
                    throw new Exception("CQRS or Request Direcotry not found");
                }

                ApplicationService.Run(fileReader);
                WebService.Run(fileReader);
            }

            if (args[0] == Commands.Test)
            {
                var root = Util.GetSolutionParentPath();
                var currentDirectory = Directory.GetCurrentDirectory();

                Console.WriteLine(@$"
                                       currentDirectory:{currentDirectory}
                                       rootPath:{root}
                                    ");
            }


        }



    }
}
