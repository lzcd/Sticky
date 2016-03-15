﻿using System;
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
            var namedNodeDescriptions = Paths.Take(Paths.Count() - 1);
            var nodeDescriptionByName = namedNodeDescriptions.ToDictionary(k => k.NodeDescription.Identifier, v => v.NodeDescription);

            var pathCriteria = Paths.Last();
            var firstNodeMatchDescription = ToGraph(pathCriteria);

            var matchingNodes = FindMatchingModes(nodes,
                                    ResolveIfNamed(nodeDescriptionByName, firstNodeMatchDescription));

            var traceHeads = new Queue<TraceNode>(from node in matchingNodes
                                                  select new TraceNode()
                                                  {
                                                      Node = node,
                                                      NodeMatchDescription = firstNodeMatchDescription
                                                  });

            while (traceHeads.Any())
            {
                var head = traceHeads.Dequeue();
                foreach (var relationshipDescription in head.NodeMatchDescription.RelationshipDescriptions)
                {
                    var relationships = default(List<Relationship>);
                    switch (relationshipDescription.Direction)
                    {
                        case RelationshipDirection.Left:
                            relationships = head.Node.IncomingRelationships;
                            break;
                        case RelationshipDirection.Right:
                            relationships = head.Node.OutgoingRelationships;
                            break;
                    }

                    var otherNodeDescription = ResolveIfNamed(nodeDescriptionByName, relationshipDescription.Node);

                    foreach (var relationship in relationships)
                    {
                        if (!DoesMatchDescription(relationship, relationshipDescription))
                        {
                            continue;
                        }

                        var otherNode = relationship.Node;
                        if (!DoesMatchDescription(otherNode, otherNodeDescription))
                        {
                            continue;
                        }


                        var newHead = new TraceNode()
                        {
                            Node = otherNode,
                            NodeMatchDescription = relationshipDescription.Node,
                            PreviousRelationship = relationship,
                            PreviousTraceNode = head
                        };
                        traceHeads.Enqueue(newHead);
                    }
                }
            }
        }

        private static List<Node> FindMatchingModes(List<Node> nodes, NodeMatchDescription nodeDescription)
        {
            var matchingNodes = new List<Node>();
            foreach (var node in nodes)
            {
                if (!DoesMatchDescription(node, nodeDescription))
                {
                    continue;
                }
                matchingNodes.Add(node);
            }
            return matchingNodes;
        }

        private static NodeMatchDescription ResolveIfNamed(Dictionary<string, NodeMatchDescription> nodeDescriptionByName, NodeMatchDescription nodeDescription)
        {
            if (!String.IsNullOrEmpty(nodeDescription.Identifier))
            {
                return nodeDescriptionByName[nodeDescription.Identifier];
            }

            return nodeDescription;
        }

        private static NodeMatchDescription ToGraph(PathMatchDescription pathCriteria)
        {
            var startNode = pathCriteria.NodeDescription;
            var currentNode = startNode;

            // TODO: deal with depth counts
            foreach (var connectionDescription in pathCriteria.ConnectionDescriptions)
            {
                if (connectionDescription.RelationshipDescription.DepthRange == null)
                {
                    var relationshipDescription = connectionDescription.RelationshipDescription;
                    relationshipDescription.Node = connectionDescription.NodeDescription;
                    currentNode.RelationshipDescriptions.Add(relationshipDescription);
                    currentNode = relationshipDescription.Node;
                    continue;

                }

                var startingCurrentNode = currentNode;
                var repeatedRelationshipDescription = connectionDescription.RelationshipDescription;
                for (var depth = repeatedRelationshipDescription.DepthRange.Minimum;
                    depth <= repeatedRelationshipDescription.DepthRange.Maximum;
                    depth++)
                {
                    currentNode = startingCurrentNode;
                    for (var currentDepth = 1;
                        currentDepth < depth;
                        currentDepth++)
                    {
                        var relationshipDescription = repeatedRelationshipDescription.Clone();
                        relationshipDescription.Node = new NodeMatchDescription();
                        currentNode.RelationshipDescriptions.Add(relationshipDescription);
                        currentNode = relationshipDescription.Node;
                    }

                    var tailRelationshipDescription = repeatedRelationshipDescription.Clone();
                    tailRelationshipDescription.Node = connectionDescription.NodeDescription;
                    currentNode.RelationshipDescriptions.Add(tailRelationshipDescription);
                    currentNode = tailRelationshipDescription.Node;
                }
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
