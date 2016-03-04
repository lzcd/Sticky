using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace Sticky.Cypher
{
    class RelationshipMatchDescription
    {
        public RelationshipDirection Direction { get; internal set; }
        public IEnumerable<string> Labels { get; internal set; }
        public DepthRangeDescription DepthRange { get; internal set; }
        public IEnumerable<PropertyDescription> PropertyDescriptions { get; internal set; }
        public NodeMatchDescription Node { get; internal set; }
    }
}
