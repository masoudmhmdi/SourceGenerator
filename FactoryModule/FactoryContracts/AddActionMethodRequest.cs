using SourceGenerator.Constant.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceGenerator.FactoryModule.FactoryContracts
{
    public class AddActionMethodRequest
    {
        public  string Verb { get; set; }
        public  string Name { get; set; }

        public AddActionMethodRequest(HttpVerb verb, string name)
        {
            Name = name;

            Verb = verb switch
            {
                HttpVerb.DELETE => "Delete",
                HttpVerb.PUT => "Put",
                HttpVerb.POST => "Post",
                HttpVerb.GET => "Get",
                _ => string.Empty,
            };

        }   
    }



}
