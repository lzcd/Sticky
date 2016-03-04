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
        public string Identifier { get; set; }
        public string Label { get; set; }
        public IEnumerable<PropertyDescription> PropertyDescriptions { get; set; }
        public List<RelationshipMatchDescription> RelationshipDescriptions { get; } = new List<RelationshipMatchDescription>();
    }
}
