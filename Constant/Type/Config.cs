using SourceGenerator.Constant.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator.Constant.Type
{
    public class Config
    {
        public required string _controllerPath;
        public required string _controllerName;
        public required string _apiName;
        public required string _requestPath;
        public required string _responsePath;
        public required string _verb;
        public required string _CQRSPath;
    }
}
