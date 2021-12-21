using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Generic
    {
        public Generic(string typeName, params Generic[] generics)
        {
            TypeName = typeName;
            GenericTypes = generics;
        }

        public string TypeName { get; set; }
        public IEnumerable<Generic> GenericTypes { get; set; } = new List<Generic>();

        public static implicit operator Generic(string Name) => new(Name);

        public static Generic Of(string name) => new(name);
        public static Generic Of(string name, params Generic[] generics) => new(name, generics);
    }
}
