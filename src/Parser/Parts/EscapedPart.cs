using System.Diagnostics;

namespace Parser
{
    [DebuggerDisplay("{Describe}")]
    public class EscapePart : IPart
    {
        public static readonly EscapePart OpenBrace = new EscapePart("{", "{{");
        public static readonly EscapePart CloseBrace = new EscapePart("}", "}}");

        private readonly string _text, _escaped;

        private EscapePart(string text, string escaped)
        {
            _text = text; 
            _escaped = escaped;
        }

        private string Describe => "Text: " + _text;

        public string Print() => _escaped;         
    }
}