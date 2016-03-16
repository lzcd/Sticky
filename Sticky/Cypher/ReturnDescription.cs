using System.Collections.Generic;

namespace Sticky.Cypher
{
    class ReturnDescription
    {
        public bool Distinct { get; internal set; }
        public IEnumerable<ReturnProjectionDescription> Projections { get; set; }
    }
}