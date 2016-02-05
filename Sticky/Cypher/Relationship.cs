using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace Sticky.Cypher
{
    class Relationship
    {
        public string Label { get; internal set; }
        public IEnumerable<Property> Properties { get; internal set; }
    }
}
