using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Serialization;

namespace Smartunicom.Runtime.Serialization.ContractResolver
{
    public class JsonSnakeCasePropertyNamesContractResolver : DefaultContractResolver
    {
        public JsonSnakeCasePropertyNamesContractResolver() : base()
        {

        }

        protected override string ResolvePropertyName(string name)
        {
            for(int i = name.Length - 1; i > 0; i--)
            {
                if(i > 0 && char.IsUpper(name[i]))
                {
                    name = name.Insert(i, "_");
                }
            }

            return name.ToLower();
        }
    }
}
