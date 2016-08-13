using System.Diagnostics;
using System.Text;

namespace Parser
{

    [DebuggerDisplay("Escaped Text: {Escaped}")]
    public class EscapePart : IPart
    {
        public static readonly EscapePart OpenBrace = new EscapePart("{", "{{");
        public static readonly EscapePart CloseBrace = new EscapePart("}", "}}");

        /// <summary>
        /// Text without escape chars
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Escaped representation of <see cref="Text"/>
        /// </summary>
        public string Escaped { get; }

        private EscapePart(string text, string escaped)
        {
            Text = text; 
            Escaped = escaped;
        }

        public string Print() => Escaped;

        public int RenderPart(StringBuilder sb, Renderer renderer, int argIndex, object[] args)
        {
            sb.Append(Text);
            return argIndex;
        }

        public void RenderPartIndexed(StringBuilder sb, Renderer renderer, object[] args)
        {
            sb.Append(Text);
        }
    }
}