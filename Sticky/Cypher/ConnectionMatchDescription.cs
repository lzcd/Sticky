using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class ConnectionMatchDescription
    {
        public NodeMatchDescription NodeDescription { get; internal set; }
        public RelationshipMatchDescription RelationshipDescription { get; internal set; }
    }
}
