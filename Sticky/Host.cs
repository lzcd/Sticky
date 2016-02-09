
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky
{
    public class Host
    {
        private List<Node> nodes = new List<Node>();

        public void Execute(string source)
        {
            var parser = new Cypher.Parser();
            var command = parser.ToAst(source);
            command.Apply(nodes);
        }
    }
}
