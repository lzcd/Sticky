using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace Sticky.Cypher
{
    class NodeDescription
    {
        public string Identifier { get; set; }
        public string Label { get; set; }
        public IEnumerable<PropertyDescription> PropertyDescriptions { get; set; }
    }
}
