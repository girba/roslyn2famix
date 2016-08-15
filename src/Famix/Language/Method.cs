using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Famix.Language
{
    public class Method
    {
        private readonly string name;

        public Method(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return $"(Method {this.name})";
        }
    }
}
