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

    class RelationshipDescription
    {
        public string Label { get; set; }
        public IEnumerable<PropertyDescription> PropertyDescriptions { get; set; }
        public RelationshipDirection Direction { get; set; }
    }
}
