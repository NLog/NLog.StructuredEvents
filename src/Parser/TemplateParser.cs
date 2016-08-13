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

        private static readonly char[] HoleDelimiters = { ',', ':', '}' };
        private static readonly char[] AlignmentDelimiters = { ':', '}' };
        private static readonly char[] TextDelimiters = { '{', '}' };
        private readonly string _template;
        private readonly int _length;
        private int _position;
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
                    ParseHole(HoleType.Normal);
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
            int position;
            string name = ParseName(out position); 
            int? aligment = Peek() == ',' ? ParseAlignment() : (int?)null;
            string format = Peek() == ':' ? ParseFormat() : null;
            Skip('}');
            _parts.Add(new HolePart(name, type, position, format, aligment));
        }

        /// <summary>
        /// Parse hole name
        /// </summary>
        /// <param name="parameterIndex"></param>
        /// <returns></returns>
        private string ParseName(out int parameterIndex)
        {    
            parameterIndex = -1;        
            char c = Peek();
            // If the name matches /^\d+ *$/ we consider it positional
            if (c >= '0' && c <= '9')
            {
                int start = _position;
                int parsed = ReadInt();
                SkipSpaces();
                c = Peek();
                if (c == '}' || c == ':' || c == ',')
                    parameterIndex = parsed;
                _position = start;        
            }

            if (parameterIndex == -1)
            {
                if (_parts.IsPositional == true)
                {
                    throw new TemplateParserException("Expected a numeric hole", _position);
                }

                _parts.IsPositional = false;
            }
            else
            {
                if (_parts.IsPositional == false)
                {
                    throw new TemplateParserException("Expected a named hole", _position);
                }
                _parts.IsPositional = true;
            }
               

            return ReadUntil(HoleDelimiters);            
        }

        private string ParseFormat()
        {
            Skip(':');
            // TODO: Escaped }} in formats?
            return ReadUntil('}');
        }

        private int ParseAlignment()
        {
            Skip(',');
            int i = ReadInt();
            char next = Peek();
            if (next != ':' && next != '}')
                throw new TemplateParserException($"Expected ':' or '}}' but found '{next}' instead.", _position);
            return i;
        }

        private char Peek() => _template[_position];

        private char Read() => _template[_position++];

        private void Skip(char c)
        {
            // Can be out of bounds, but never in correct use (expects a required char).
            Assert(_template[_position] == c);
            _position++;
        }

        private void SkipSpaces()
        {
            // Can be out of bounds, but never in correct use (inside a hole).
            while (_template[_position] == ' ') _position++;
        }

        private int ReadInt()
        {               
            SkipSpaces();                                
                        
            bool negative = false;
            if (Peek() == '-')
            {
                negative = true;
                _position++;
            }

            int i = 0;
            bool hasDigits = false;
            while (true) 
            {
                // Can be out of bounds, but never in correct use (inside a hole).
                char c = Peek();
                int digit = c - '0';
                if (digit < 0 || digit > 9) break;
                hasDigits = true;
                _position++;
                i = i * 10 + digit;
            }
            if (!hasDigits) 
                throw new TemplateParserException("An integer is expected", _position);
            
            SkipSpaces();
            return negative ? -i : i;
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
