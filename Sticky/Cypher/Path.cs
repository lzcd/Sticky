using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class Path
    {
        public Node LeftNode { get; internal set; }
        public Relationship Relationship { get; internal set; }
        public Node RightNode { get; internal set; }
    }
}
