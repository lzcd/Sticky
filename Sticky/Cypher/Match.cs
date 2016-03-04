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
            var nodeDescriptionByName = namedDescription.ToDictionary(k => k.NodeDescription.Identifier, v => v.NodeDescription);

            var pathCriteria = Paths.Last();
            var startNode = ToGraph(pathCriteria);
            var currentNode = startNode;

            var nodeDescription = currentNode;
            if (!String.IsNullOrEmpty(nodeDescription.Identifier))
            {
                nodeDescription = nodeDescriptionByName[nodeDescription.Identifier];
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

            var relationshipDescription = currentNode.RelationshipDescriptions.First();

            foreach (var matchingNode in matchingNodes)
            {
                var relationships = default(IEnumerable<Relationship>);
                switch (relationshipDescription.Direction)
                {
                    case RelationshipDirection.Left:
                        relationships = matchingNode.IncomingRelationships;
                        break;
                    case RelationshipDirection.Right:
                        relationships = matchingNode.OutgoingRelationships;
                        break;
                    default:
                        throw new Exception();
                }


                var matchingRelationships = new List<Relationship>();
                foreach (var relationship in relationships)
                {
                    if (!DoesMatchDescription(relationship, relationshipDescription))
                    {
                        continue;
                    }
                    matchingRelationships.Add(relationship);
                }

                var matchingOtherNodes = new List<Node>();
                foreach (var relationship in matchingRelationships)
                {
                    var otherNode = relationship.Node;
                    if (!DoesMatchDescription(otherNode, relationshipDescription.Node))
                    {
                        continue;
                    }
                    matchingOtherNodes.Add(otherNode);
                }
            }
        }

        private static NodeMatchDescription ToGraph(PathMatchDescription pathCriteria)
        {
            var startNode = pathCriteria.NodeDescription;
            var currentNode = startNode;

            foreach (var connection in pathCriteria.ConnectionDescriptions)
            {
                var rd = connection.RelationshipDescription;
                rd.Node = connection.NodeDescription;
                currentNode.RelationshipDescriptions.Add(rd);
                currentNode = rd.Node;
            }

            return startNode;
        }

        private static bool DoesMatchDescription(Relationship relationship, RelationshipMatchDescription description)
        {
            if (!description.Labels.Contains(relationship.Label))
            {
                return false;
            }

            foreach (var propertyDescription in description.PropertyDescriptions)
            {
                if (!DoesMatchDescription(relationship.PropertyByName, propertyDescription))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool DoesMatchDescription(Node node, NodeMatchDescription description)
        {
            if (node.Label != description.Label)
            {
                return false;
            }

            foreach (var propertyDescription in description.PropertyDescriptions)
            {
                if (!DoesMatchDescription(node.PropertyByName, propertyDescription))
                {
                    return false;
                }
            }

            return true;
        }


        private static bool DoesMatchDescription(Dictionary<string, HasValue> propertyByName, PropertyDescription propertyDescription)
        {
            var name = propertyDescription.Name;

            var testValue = default(HasValue);
            if (!propertyByName.TryGetValue(name, out testValue))
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
                return false;
            }

            return true;
        }
    }
}
