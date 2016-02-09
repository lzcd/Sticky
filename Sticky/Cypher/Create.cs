using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sticky.Cypher
{
    class Create
    {
        public IEnumerable<PathDescription> Paths;

        public void Apply(List<Node> existingNodes)
        {
            var nodeByIdentifier = new Dictionary<string, Node>();

            foreach(var path in Paths)
            {
                ToNode(path, nodeByIdentifier);
            }
            existingNodes.AddRange(nodeByIdentifier.Values);
        }

        private static Node ToNode(PathDescription path, Dictionary<string, Node> nodeByIdentifier)
        {
            var nodeDescription = path.NodeDescription;

            var node = FindOrCreateNode(nodeDescription, nodeByIdentifier);

            if (!string.IsNullOrEmpty(nodeDescription.Label))
            {
                node.Label = nodeDescription.Label;
            }
            foreach (var propertyDescription in nodeDescription.PropertyDescriptions)
            {
                ApplyProperty(propertyDescription, node);
            }

            if (path.ConnectionDescription != null)
            {
                var otherNodeDescription = path.ConnectionDescription.NodeDescription;
                var otherNode = FindOrCreateNode(otherNodeDescription, nodeByIdentifier);

                var relationshipDescription = path.ConnectionDescription.RelationshipDescription;
                var relationship = new Relationship {
                    Label = relationshipDescription.Label,
                    Node = otherNode
                };

                foreach (var propertyDescription in relationshipDescription.PropertyDescriptions)
                {
                    ApplyProperty(propertyDescription, relationship);
                }

                node.Relationships.Add(relationship);
            }
            return node;
        }

        private static void ApplyProperty(PropertyDescription propertyDescription, HasProperties container)
        {
            var name = propertyDescription.Name;
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

            container.PropertyByName.Add(name, value);
        }

        private static Node FindOrCreateNode(NodeDescription nodeDescription, Dictionary<string, Node> nodeByIdentifier)
        {
            var node = default(Node);
            if (!nodeByIdentifier.TryGetValue(nodeDescription.Identifier, out node))
            {
                node = new Node();
                nodeByIdentifier.Add(nodeDescription.Identifier, node);
            }

            return node;
        }
    }
}
