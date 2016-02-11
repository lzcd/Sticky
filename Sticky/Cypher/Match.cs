using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class Match : Applier
    {
        public IEnumerable<PathMatchDescription> Paths { get; set; }

        public void Apply(List<Node> nodes)
        {
            var anchorCriteria = Paths.Take(Paths.Count() - 1);
            var path = Paths.Last();

            var anchorMatchesByIdentifier = new Dictionary<string, List<Node>>();
            foreach(var anchorCriterion in anchorCriteria)
            {
                foreach(var node in nodes)
                {
                    if (node.Label != anchorCriterion.NodeDescription.Label)
                    {
                        continue;
                    }

                    foreach(var propertyPairCriterion in anchorCriterion.NodeDescription.PropertyDescriptions)
                    {
                        var testValue = default(HasValue);
                        if (!node.PropertyByName.TryGetValue(propertyPairCriterion.Name, out testValue))
                        {
                            continue;
                        }

                        if (testValue != propertyPairCriterion)
                        {
                            continue;
                        }
                    }

                    var identifier = anchorCriterion.NodeDescription.Identifier;
                    var anchorMatches = default(List<Node>);
                    if (!anchorMatchesByIdentifier.TryGetValue(identifier, out anchorMatches))
                    {
                        anchorMatches = new List<Node>();
                        anchorMatchesByIdentifier.Add(identifier, anchorMatches);
                    }
                    anchorMatches.Add(node);
                }
            }
        }
    }
}
