using System;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Parser
{
    public class TemplateParser
    {
        public static PartList Parse(string template)
        {
            if (template == null) 
                throw new ArgumentNullException(nameof(template));
            var parser = new TemplateParser(template);
            return parser.Parse();
        }

        private static readonly char[] HoleDelimiters = { ':', '}' };
        private static readonly char[] TextDelimiters = { '{', '}' };
        private readonly string _template;
        private readonly int _length;
        private int _position;
        private int _holeIndex;
        private PartList _parts;

        private TemplateParser(string template)
        {
            _template = template;
            _length = template.Length;
        }

        private PartList Parse()
        {
            try
            {
                _parts = new PartList();
                while (_position < _length)
                {
                    char c = Peek();
                    if (c == '{')
                        ParseOpenBracketPart();
                    else if (c == '}')
                        ParseCloseBracketPart();
                    else
                        ParseTextPart();
                }
                return _parts;
            }
            catch (IndexOutOfRangeException)
            {
                throw new TemplateParserException("Unexpected end of template.", _position);
            }
        }

        private void ParseTextPart()
        {
            string text = ReadUntil(TextDelimiters, required: false);
            _parts.Add(new TextPart(text));
        }

        private void ParseOpenBracketPart()
        {
            Skip('{');
            char c = Peek();
            switch (c)
            {
                case '{':
                    Skip('{');
                    _parts.Add(EscapePart.OpenBrace);
                    return;
                case '@':
                    Skip('@');
                    ParseHole(HoleType.Destructuring);
                    return;
                case '$':
                    Skip('$');
                    ParseHole(HoleType.Stringification);
                    return;
                default:
                    // TODO: "0a" is wrong of course
                    ParseHole(c >= '0' && c <= '9' ? HoleType.Numeric : HoleType.Text);
                    return;
            }
        }

        private void ParseCloseBracketPart()
        {
            Skip('}');
            if (Read() != '}')
                throw new TemplateParserException($"Unexpected '}}' ", _position-2);
            _parts.Add(EscapePart.CloseBrace);
        }

        private void ParseHole(HoleType type)
        {
            string name = ReadUntil(HoleDelimiters);
            string format = Peek() == ':' ? ParseFormat() : null;
            Skip('}');
            _parts.Add(new HolePart(name, type, _holeIndex++, format));
        }

        private string ParseFormat()
        {
            Skip(':');
            // TODO: Escaped }} in formats?
            return ReadUntil('}');
        }

        private char Peek() => _template[_position];

        private char Read() => _template[_position++];

        private void Skip(char c)
        {
            Assert(_template[_position] == c);
            _position++;
        }

        private string ReadUntil(char search, bool required = true)
        {
            int start = _position;
            int i = _template.IndexOf(search, _position);
            if (i == -1 && required)
                throw new TemplateParserException($"Reached end of template while expecting '{search}'.", _position);
            _position = i == -1 ? _length : i;
            return _template.Substring(start, _position - start);
        }

        private string ReadUntil(char[] search, bool required = true)
        {
            int start = _position;
            int i = _template.IndexOfAny(search, _position);
            if (i == -1 && required)
            {
                var formattedChars = string.Join(", ", search.Select(c => "'" + c + "'").ToArray()); 
                throw new TemplateParserException($"Reached end of template while expecting one of {formattedChars}.", _position);
            }
            _position = i == -1 ? _length : i;
            return _template.Substring(start, _position - start);
        }
    }
}