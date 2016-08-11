using System;
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

        private static readonly char[] HoleDelimiters = new[] { ':', '}' };
        private static readonly char[] TextDelimiters = new[] { '{', '}' };
        private readonly string template;
        private readonly int len;
        private int pos = 0, holeIndex = 0;
        private PartList parts = null;

        private TemplateParser(string template)
        {
            this.template = template;
            this.len = template.Length;
        }

        private PartList Parse()
        {
            try
            {
                parts = new PartList();
                while (pos < len)
                {
                    char c = Peek();
                    if (c == '{')
                        ParseOpenBracketPart();
                    else if (c == '}')
                        ParseCloseBracketPart();
                    else
                        ParseTextPart();
                }
                return parts;
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new Exception("Unexpected end of template", ex);  // TODO
            }
        }

        private void ParseTextPart()
        {
            string text = ReadUntil(TextDelimiters, required: false);
            parts.Add(new TextPart(text));
        }

        private void ParseOpenBracketPart()
        {
            Skip('{');
            char c = Peek();
            switch (c)
            {
                case '{':
                    Skip('{');
                    parts.Add(new TextPart('{', '{'));
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
                throw new Exception("Parse error"); // TODO
            parts.Add(new TextPart('}', '}'));
        }

        private void ParseHole(HoleType type)
        {
            string name = ReadUntil(HoleDelimiters);
            string format = Peek() == ':' ? ParseFormat() : null;
            Skip('}');
            parts.Add(new HolePart(name, type, holeIndex++, format));
        }

        private string ParseFormat()
        {
            Skip(':');
            return ReadUntil('}');
        }

        private char Peek() => template[pos];

        private char Read() => template[pos++];

        private void Skip(char c)
        {
            Assert(template[pos] == c);
            pos++;
        }

        private string ReadUntil(char search, bool required = true)
        {
            int start = pos;
            int i = template.IndexOf(search, pos);
            if (i == -1 && required)
                throw new Exception("Missing char");  // TODO
            pos = i == -1 ? len : i;
            return template.Substring(start, pos - start);
        }

        private string ReadUntil(char[] search, bool required = true)
        {
            int start = pos;
            int i = template.IndexOfAny(search, pos);
            if (i == -1 && required)
                throw new Exception("Missing char");  // TODO
            pos = i == -1 ? len : i;
            return template.Substring(start, pos - start);
        }
    }
}