using System;
using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

namespace Parser
{
    public class TemplateParser
    {
        public static Template Parse(string template)
        {
            if (template == null) 
                throw new ArgumentNullException(nameof(template));
            var parser = new TemplateParser(template);
            return parser.Parse();
        }

        private static readonly char[] HoleDelimiters = { ',', ':', '}' };
        private static readonly char[] TextDelimiters = { '{', '}' };
        private List<Literal> _literals = new List<Literal>();
        private List<Hole> _holes = new List<Hole>();
        private bool _isPositional = true;
        private string _template;
        private int _length;
        private int _position;
        private int _literalLength;

        private TemplateParser(string template)
        {
            _template = template;
            _length = template.Length;
        }

        // Literals are added to the result list when:
        // - a hole is encountered
        // - an escape is encountered. The escape itself is always included in the preceding literal.
        // - at the end of the template.
        private void AddLiteral(int skip = 0)
        {
            _literals.Add(new Literal { Print = (ushort)_literalLength, Skip = (ushort)skip });
            _literalLength = 0;
        }

        private Template Parse()
        {
            try
            {
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
                if (_literalLength != 0)
                    AddLiteral();

                Assert(_holes.Count == _literals.Count(x => x.Skip > 0));
                return new Template(_template, _isPositional, _literals, _holes);
            }
            catch (IndexOutOfRangeException)
            {
                throw new TemplateParserException("Unexpected end of template.", _position);
            }
        }

        private void ParseTextPart()
        {
            _literalLength = (ushort)SkipUntil(TextDelimiters, required: false);
        }

        private void ParseOpenBracketPart()
        {
            Skip('{');
            char c = Peek();
            switch (c)
            {
                case '{':
                    Skip('{');
                    _literalLength++;
                    AddLiteral();                    
                    return;
                case '@':
                    Skip('@');
                    ParseHole(CaptureType.Destructuring);
                    return;
                case '$':
                    Skip('$');
                    ParseHole(CaptureType.Stringification);
                    return;
                default:
                    ParseHole(CaptureType.Normal);
                    return;
            }
        }

        private void ParseCloseBracketPart()
        {
            Skip('}');
            if (Read() != '}')
                throw new TemplateParserException($"Unexpected '}}' ", _position - 2);
            _literalLength++;
            AddLiteral();
        }

        private void ParseHole(CaptureType type)
        {
            int start = _position;
            int position;
            string name = ParseName(out position); 
            int alignment = Peek() == ',' ? ParseAlignment() : 0;
            string format = Peek() == ':' ? ParseFormat() : null;
            Skip('}');
            AddLiteral(_position - start + (type == CaptureType.Normal ? 1 : 2));   // Account for skipped '{', '{$' or '{@'
            _holes.Add(new Hole 
            { 
                Name = name,
                Format = format,
                CaptureType = type,
                Index = (byte)position,
                Alignment = (short)alignment,
            });
        }

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
                _isPositional = false;

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

        private int SkipUntil(char[] search, bool required = true)
        {
            int start = _position;
            int i = _template.IndexOfAny(search, _position);
            if (i == -1 && required)
            {
                var formattedChars = string.Join(", ", search.Select(c => "'" + c + "'").ToArray()); 
                throw new TemplateParserException($"Reached end of template while expecting one of {formattedChars}.", _position);
            }
            _position = i == -1 ? _length : i;
            return _position - start;
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
            return _template.Substring(start, SkipUntil(search, required));
        }
    }
}