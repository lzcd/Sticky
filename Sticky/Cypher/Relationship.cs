using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace Sticky.Cypher
{
    enum RelationshipDirection
    {
        Unknown,
        Right,
        Left
    }

    class Relationship
    {
        public string Label { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public RelationshipDirection Direction { get; set; }
    }
}
