using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace Sticky.Cypher
{
    class NodeMatchDescription
    {
        public string Identifier { get; internal set; }
        public string Label { get; internal set; }
        public IEnumerable<PropertyDescription> PropertyDescriptions { get; internal set; }
    }
}
