
using Sticky.Cypher;
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

        public ResultTable Execute(string source)
        {
            var parser = new Cypher.Parser();
            var command = parser.ToAst(source);
            var applierResult = command.Apply(nodes);
            return applierResult.Result;
        }
    }
}
