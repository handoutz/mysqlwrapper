using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLWrapper
{
    public struct MySQLParameterPair
    {
        public string Name { get; set; }
        public object Object { get; set; }

        public MySQLParameterPair(string name, object o)
            : this()
        {
            Name = name;
            Object = o;
        }
    }
}
