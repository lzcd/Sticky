using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky
{
    class Relationship : HasLabel, HasProperties
    {
        public string Label { get; set; }

        public Node Node { get; internal set; }
        public Dictionary<string, HasValue> PropertyByName { get; } = new Dictionary<string, HasValue>();
    }
}
