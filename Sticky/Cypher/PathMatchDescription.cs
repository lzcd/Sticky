using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class PathMatchDescription
    {
        public NodeMatchDescription NodeDescription { get; internal set; }
        public IEnumerable<ConnectionMatchDescription> ConnectionDescriptions { get; internal set; }
    }
}
