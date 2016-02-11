using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class Match : Applier
    {
        public IEnumerable<PathMatchDescription> Paths { get; internal set; }

        public void Apply(List<Node> nodes)
        {
            throw new NotImplementedException();
        }
    }
}
