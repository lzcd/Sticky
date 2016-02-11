using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class PathMatchDescription
    {
        public ConnectionMatchDescription ConnectionDescription { get; internal set; }
        public NodeMatchDescription NodeDescription { get; internal set; }
    }
}
