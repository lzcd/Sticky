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
            var namedDescription = Paths.Take(Paths.Count() - 1);
            var descriptionByName = namedDescription.ToDictionary(k => k.NodeDescription.Identifier, v => v.NodeDescription);

            var pathCriteria = Paths.Last();
            var nodeDescription = pathCriteria.NodeDescription;
            if (!String.IsNullOrEmpty(nodeDescription.Identifier))
            {
                nodeDescription = descriptionByName[nodeDescription.Identifier];
            }
            var matchingNodes = new List<Node>();
            foreach (var node in nodes)
            {
                if (!DoesMatchDescription(node, nodeDescription))
                {
                    continue;
                }
                matchingNodes.Add(node);
            }
            var connectionDescription = pathCriteria.ConnectionDescriptions.First();
            var relationshipDescription = connectionDescription.RelationshipDescription;
            switch (relationshipDescription.Direction)
            {
                case RelationshipDirection.Left:

                    break;
                case RelationshipDirection.Right:
                    break;
                default:
                    throw new Exception();
            }
        }


        private static bool DoesMatchDescription(Node node, NodeMatchDescription description)
        {
            if (node.Label != description.Label)
            {
                return false;
            }

            var unmatchedPropertyFound = false;
            foreach (var propertyDescription in description.PropertyDescriptions)
            {
                var name = propertyDescription.Name;


                var testValue = default(HasValue);
                if (!node.PropertyByName.TryGetValue(name, out testValue))
                {
                    return false;
                }


                var value = default(HasValue);

                if (propertyDescription.TextValue.StartsWith("'") &&
                    propertyDescription.TextValue.EndsWith("'"))
                {
                    var textValue = propertyDescription.TextValue.Substring(1, propertyDescription.TextValue.Length - 2);
                    value = new Text { Value = textValue };
                }
                else
                {
                    var numericValue = default(decimal);
                    Decimal.TryParse(propertyDescription.TextValue, out numericValue);
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
                return false;
            }

            return true;
        }
    }
}
