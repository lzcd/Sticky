using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class TraceNode
    {
        public NodeMatchDescription NodeMatchDescription { set; get; }
        public Node Node { get; set; }
        public Relationship PreviousRelationship { get; set; }
        public TraceNode PreviousTraceNode { get; set; }
    }
}
