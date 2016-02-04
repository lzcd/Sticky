using Sprache;
using System.Collections.Generic;
using System.Linq;

namespace Sticky.Cypher
{
    public static class Parser
    {
        static readonly Parser<string> Identifier =
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
            select new string(first.Concat(rest).ToArray());

        static readonly Parser<string> Label =
            from colon in Parse.Char(':')
            from identifer in Identifier
            select identifer;

        static readonly Parser<string> QuotedText =
            from openQuote in Parse.Char('\'')
            from text in Parse.AnyChar
            from closeQuote in Parse.Char('\'')
            select string.Concat(openQuote, text, closeQuote);

        static readonly Parser<string> Number =
            Parse.Decimal;

        static readonly Parser<Property> Property =
            from leading in Parse.WhiteSpace.Many()
            from name in Identifier
            from sperator in Parse.Char(':')
            from textValue in Parse.Or(QuotedText, Number)
            select new Property { Name = name, TextValue = textValue };

        static readonly Parser<IEnumerable<Property>> Properties =
            from leading in Parse.WhiteSpace.Many()
            from openBrace in Parse.Char('{')
            from properties in Property.DelimitedBy(Parse.Char(','))
            from closeBrace in Parse.Char('}')
            select properties;

        static readonly Parser<Node> Node =
            from startNodeChar in Parse.Char('(')
            from identifier in Identifier
            from label in Parse.Optional(Label)
            from properties in Parse.Optional(Properties)
            from endNodeChar in Parse.Char(')')
            select new Node {
                Identifier = identifier,
                Label = label.IsDefined ? label.Get() : "",
                Properties = properties.IsDefined ? properties.Get() : new List<Property>() };

        static readonly Parser<Relationship> Relationship =
            from left in Parse.Optional(Parse.Char('<'))
            from leftLine in Parse.Char('-')
            from openBracket in Parse.Char('[')
            from label in Label
            from closeBracket in Parse.Char(']')
            from rightLine in Parse.Char('-')
            from right in Parse.Optional(Parse.Char('>'))
            select new Relationship { Label = label };


        static readonly Parser<Path> Path =
            from leading in Parse.WhiteSpace.Many()
            from leftNode in Node
            from relationship in Relationship
            from rightNode in Node
            select new Path { Node = leftNode };

        static readonly Parser<Command> Command =
            from leading in Parse.WhiteSpace.Many()
            from text in Identifier
            from paths in Path.DelimitedBy(Parse.Char(','))
            select new Command { Text = text, Paths = paths };

        public static void Boop(string source)
        {
            var bob = Command.Parse(source);
        }
    }
}
