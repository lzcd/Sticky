using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace Sticky.Cypher
{
    class Parser
    {
        public Applier ToAst(string source)
        {

            var identifierParser =
               from first in Parse.Letter.Once()
               from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
               select new string(first.Concat(rest).ToArray());

            var labelPrefixParser = Parse.Char(':');

            var labelParser =
                from labelPrefix in labelPrefixParser
                from identifer in identifierParser
                select identifer;

            var singleQuoteDelimiterParser = Parse.Char('\'');

            var quotedTextParser =
                from openQuote in singleQuoteDelimiterParser
                from text in Parse.AnyChar.Except(singleQuoteDelimiterParser).Many()
                from closeQuote in Parse.Char('\'')
                select string.Concat(openQuote, new string(text.ToArray()), closeQuote);

            var numberParser = Parse.Decimal;

            var valuePrefixParser = Parse.Char(':');

            var propertyParser =
                from leading in Parse.WhiteSpace.Many()
                from name in identifierParser
                from valuePrefix in valuePrefixParser
                from textValue in Parse.Or(quotedTextParser, numberParser)
                select new PropertyDescription { Name = name, TextValue = textValue };

            var propertiesParser =
                from leading in Parse.WhiteSpace.Many()
                from openBrace in Parse.Char('{')
                from properties in propertyParser.DelimitedBy(Parse.Char(','))
                from closeBrace in Parse.Char('}')
                select properties;

            var nodeParser =
                from startNodeChar in Parse.Char('(')
                from identifier in identifierParser
                from label in labelParser.Optional()
                from properties in propertiesParser.Optional()
                from endNodeChar in Parse.Char(')')
                select new NodeDescription
                {
                    Identifier = identifier,
                    Label = label.IsDefined ? label.Get() : "",
                    PropertyDescriptions = properties.IsDefined ? properties.Get() : new List<PropertyDescription>()
                };


            var relationshipParser =
                from leftDirection in Parse.Char('<').Optional()
                from leftLine in Parse.Char('-')
                from openBracket in Parse.Char('[')
                from label in labelParser
                from properties in propertiesParser.Optional()
                from closeBracket in Parse.Char(']')
                from rightLine in Parse.Char('-')
                from rightDirection in Parse.Char('>').Optional()
                select new RelationshipDescription
                {
                    Label = label,
                    PropertyDescriptions = properties.IsDefined ? properties.Get() : new List<PropertyDescription>(),
                    Direction = leftDirection.IsDefined ?
                        RelationshipDirection.Left :
                        (rightDirection.IsDefined ? RelationshipDirection.Right : RelationshipDirection.Unknown)
                };

            var connectionParser =
                from relationship in relationshipParser
                from rightNode in nodeParser
                select new ConnectionDescription { RelationshipDescription = relationship, NodeDescription = rightNode };

            var pathParser =
                from leading in Parse.WhiteSpace.Many()
                from leftNode in nodeParser
                from connection in connectionParser.Optional()
                select new PathDescription
                {
                    NodeDescription = leftNode,
                    ConnectionDescription = connection.IsDefined ? connection.Get() : null
                };

            var createCommandParser =
                from leading in Parse.WhiteSpace.Many()
                from text in Parse.String("CREATE")
                from paths in pathParser.DelimitedBy(Parse.Char(','))
                from terminator in Parse.Char(';').Optional()
                select (Applier)new Create { Paths = paths };

            var optionalPrefixParser = Parse.Char('|');

            var optionalIdentifer =
                from optionalPrefix in optionalPrefixParser
                from identifier in identifierParser
                select identifier;

            var labelMatchParser =
              from label in labelParser
              from optionalIdentifers in optionalIdentifer.Many()
              select new List<string> { label } .Concat(optionalIdentifers);

            var depthPrefixParser = Parse.Char('*');

            var depthRangeMatchParser =
                from asterix in Parse.Char('*')
                from minimumDepth in Parse.Digit.XAtLeastOnce()
                from range in Parse.Char('.').Repeat(2)
                from maximumDepth in Parse.Digit.XAtLeastOnce()
                select new DepthRangeDescription {
                    Minimum = int.Parse( new string( minimumDepth.ToArray())),
                    Maximum =  int.Parse( new string( maximumDepth.ToArray()))
                };

            var nodeMatchParser =
                from startNodeChar in Parse.Char('(')
                from identifier in identifierParser.Optional()
                from label in labelParser.Optional()
                from properties in propertiesParser.Optional()
                from endNodeChar in Parse.Char(')')
                select new NodeMatchDescription
                {
                    Identifier = identifier.IsDefined ? identifier.Get() : "",
                    Label = label.IsDefined ? label.Get() : "",
                    PropertyDescriptions = properties.IsDefined ? properties.Get() : new List<PropertyDescription>()
                };

            var relationshipMatchParser =
               from leftDirection in Parse.Char('<').Optional()
               from leftLine in Parse.Char('-')
               from openBracket in Parse.Char('[')
               from labels in labelMatchParser
               from depthRange in depthRangeMatchParser.Optional()
               from properties in propertiesParser.Optional()
               from closeBracket in Parse.Char(']')
               from rightLine in Parse.Char('-')
               from rightDirection in Parse.Char('>').Optional()
               select new RelationshipMatchDescription
               {
                   Labels = labels,
                   DepthRange = depthRange.IsDefined ? depthRange.Get() : null,
                   PropertyDescriptions = properties.IsDefined ? properties.Get() : new List<PropertyDescription>(),
                   Direction = leftDirection.IsDefined ?
                       RelationshipDirection.Left :
                       (rightDirection.IsDefined ? RelationshipDirection.Right : RelationshipDirection.Unknown)
               };

            var connectionMatchParser =
                from relationship in relationshipMatchParser
                from rightNode in nodeMatchParser
                select new ConnectionMatchDescription {
                    RelationshipDescription = relationship,
                    NodeDescription = rightNode };

            var pathMatchParser =
                from leading in Parse.WhiteSpace.Many()
                from leftNode in nodeMatchParser
                from connections in connectionMatchParser.Many()
                select new PathMatchDescription
                {
                    NodeDescription = leftNode,
                    ConnectionDescriptions = connections
                };

            var returnParser =
                from leading in Parse.WhiteSpace.Many()
                from returnKeyword in Parse.String("RETURN")
                from postReturnSpacing in Parse.WhiteSpace.Many()
                from distinctKeyword in Parse.String("DISTINCT").Optional()
                from postDistinctSpacing in Parse.WhiteSpace.Many()

                select new ReturnDescription();

            var matchCommandParser =
               from leading in Parse.WhiteSpace.Many()
               from matchKeyword in Parse.String("MATCH")
               from paths in pathMatchParser.DelimitedBy(Parse.Char(','))
               from returnDescription in returnParser
               from terminator in Parse.Char(';').Optional()
               select (Applier)new Match { Paths = paths, ReturnDescription = returnDescription };

            var commandParser =
                from command in createCommandParser
                                    .XOr(matchCommandParser)
                select command;

            var result = commandParser.Parse(source);

            return result;
        }
    }
}
