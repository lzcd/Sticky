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
            var namedCriteria = Paths.Take(Paths.Count() - 1);
            var nodeCriteriaByName = FindNamedNodeCriteria(nodes, namedCriteria);

            var pathCriteria = Paths.Last();

        }

        private static Dictionary<string, List<Node>> FindNamedNodeCriteria(List<Node> nodes, IEnumerable<PathMatchDescription> anchorCriteria)
        {
            var nodeCriteriaByName = new Dictionary<string, List<Node>>();
            foreach (var anchorCriterion in anchorCriteria)
            {
                foreach (var node in nodes)
                {
                    if (node.Label != anchorCriterion.NodeDescription.Label)
                    {
                        continue;
                    }

                    var unmatchedPropertyFound = false;
                    foreach (var propertyPairCriterion in anchorCriterion.NodeDescription.PropertyDescriptions)
                    {
                        var name = propertyPairCriterion.Name;
                      

                        var testValue = default(HasValue);
                        if (!node.PropertyByName.TryGetValue(name, out testValue))
                        {
                            continue;
                        }


                        var value = default(HasValue);

                        if (propertyPairCriterion.TextValue.StartsWith("'") &&
                            propertyPairCriterion.TextValue.EndsWith("'"))
                        {
                            var textValue = propertyPairCriterion.TextValue.Substring(1, propertyPairCriterion.TextValue.Length - 2);
                            value = new Text { Value = textValue };
                        }
                        else
                        {
                            var numericValue = default(decimal);
                            Decimal.TryParse(propertyPairCriterion.TextValue, out numericValue);
                            value = new Number { Value = numericValue };
                        }

                        if (!testValue.Equals(value))
                        {
                            unmatchedPropertyFound = true;
                            break;
                        }
                    }
                    if (unmatchedPropertyFound)
                    {
                        continue;
                    }

                    var identifier = anchorCriterion.NodeDescription.Identifier;
                    var matches = default(List<Node>);
                    if (!nodeCriteriaByName.TryGetValue(identifier, out matches))
                    {
                        matches = new List<Node>();
                        nodeCriteriaByName.Add(identifier, matches);
                    }
                    matches.Add(node);
                }
            }

            return nodeCriteriaByName;
        }
    }
}
