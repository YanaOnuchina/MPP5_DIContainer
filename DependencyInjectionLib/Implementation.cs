using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionLib
{
    public class Implementation
    {
        public Type Type { get; set; }
        public bool IsSingleton { get; set; }
        public object Value { get; set; }

        public Implementation(Type type, bool isSingleton)
        {
            Type = type;
            IsSingleton = isSingleton;
        }
    }
}
