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
        public ReturnDescription ReturnDescription { get; set; }

        public void Apply(List<Node> nodes)
        {
            var namedNodeDescriptions = Paths.Take(Paths.Count() - 1);
            var nodeDescriptionByName = namedNodeDescriptions.ToDictionary(k => k.NodeDescription.Identifier, v => v.NodeDescription);

            var pathCriteria = Paths.Last();
            var firstNodeMatchDescription = ToGraph(pathCriteria);

            var matchingNodes = FindMatchingModes(nodes,
                                    ResolveIfNamed(nodeDescriptionByName, firstNodeMatchDescription));

            var matchingTraceHeads = TraceMatches(firstNodeMatchDescription, matchingNodes, nodeDescriptionByName);

            var results = ProjectToTable(matchingTraceHeads, ReturnDescription);
        }

        private static ResultTable ProjectToTable(List<TraceNode> matchingTraceHeads, ReturnDescription returnDescription)
        {
            var results = new ResultTable();
            foreach (var matchingTraceHead in matchingTraceHeads)
            {
                var rowIndex = results.AddRow();
                foreach (var projection in returnDescription.Projections)
                {
                    var columnName = projection.PropertyName;
                    if (!string.IsNullOrEmpty(projection.Alias))
                    {
                        columnName = projection.Alias;
                    }

                    var traceNode = matchingTraceHead;
                    do
                    {
                        if (traceNode.NodeMatchDescription.Identifier == projection.NodeName &&
                            traceNode.Node.PropertyByName.ContainsKey(projection.PropertyName))
                        {
                            var propertyValue = traceNode.Node.PropertyByName[projection.PropertyName];
                            results[rowIndex, columnName] = propertyValue.ToString();
                        }
                        traceNode = traceNode.PreviousTraceNode;
                    } while (traceNode != null);
                }
            }

            return results;
        }

        private static List<TraceNode> TraceMatches(NodeMatchDescription startingDescription, List<Node> startingNodes, Dictionary<string, NodeMatchDescription> nodeDescriptionByName)
        {
            var traceHeads = new Queue<TraceNode>(from node in startingNodes
                                                  select new TraceNode()
                                                  {
                                                      Node = node,
                                                      NodeMatchDescription = startingDescription
                                                  });
            var completedTraceHeads = new List<TraceNode>();

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

                        if (relationshipDescription.Node.RelationshipDescriptions.Any())
                        {
                            traceHeads.Enqueue(newHead);
                        }
                        else
                        {
                            completedTraceHeads.Add(newHead);
                        }
                    }
                }
            }

            return completedTraceHeads;
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
                var namedNodeDescription = default(NodeMatchDescription);
                if (nodeDescriptionByName.TryGetValue(nodeDescription.Identifier, out namedNodeDescription))
                {
                    return namedNodeDescription;
                }
            }

            return nodeDescription;
        }

        private static NodeMatchDescription ToGraph(PathMatchDescription pathCriteria)
        {
            var startNode = pathCriteria.NodeDescription;
            var currentNode = startNode;

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
            if (!string.IsNullOrEmpty(description.Label) && node.Label != description.Label)
            {
                return false;
            }

            if (description.PropertyDescriptions != null)
            {
                foreach (var propertyDescription in description.PropertyDescriptions)
                {
                    if (!DoesMatchDescription(node.PropertyByName, propertyDescription))
                    {
                        return false;
                    }
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
