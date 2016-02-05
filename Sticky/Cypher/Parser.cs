using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace Sticky.Cypher
{
    class Parser
    {
        public Command ToCommand(string source)
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
                select new Property { Name = name, TextValue = textValue };

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
                select new Node
                {
                    Identifier = identifier,
                    Label = label.IsDefined ? label.Get() : "",
                    Properties = properties.IsDefined ? properties.Get() : new List<Property>()
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
                select new Relationship
                {
                    Label = label,
                    Properties = properties.IsDefined ? properties.Get() : new List<Property>(),
                    Direction = leftDirection.IsDefined ?
                        RelationshipDirection.Left :
                        (rightDirection.IsDefined ? RelationshipDirection.Right : RelationshipDirection.Unknown)
                };

            var connectionParser =
                from relationship in relationshipParser
                from rightNode in nodeParser
                select new Connection { Relationship = relationship, Node = rightNode };

            var pathParser =
                from leading in Parse.WhiteSpace.Many()
                from leftNode in nodeParser
                from connection in connectionParser.Optional()
                select new Path
                {
                    Node = leftNode,
                    Connection = connection.IsDefined ? connection.Get() : null
                };

            var commandParser =
                from leading in Parse.WhiteSpace.Many()
                from text in identifierParser
                from paths in pathParser.DelimitedBy(Parse.Char(','))
                from terminator in Parse.Char(';').Optional()
                select new Command { Text = text, Paths = paths };

            var command = commandParser.Parse(source);
            return command;
        }
    }
}
