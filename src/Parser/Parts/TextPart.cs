using System.Diagnostics;
using System.Text;

namespace Parser
{
    [DebuggerDisplay("Text: {Text}")]
    public class TextPart : IPart
    {
        public string Text { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(string text)
        {
            Text = text;
        }

        #region IPart implementation

        public string Print() => Text;
        public int RenderPart(StringBuilder sb, Renderer renderer, int argIndex, object[] args)
        {
            renderer.RenderPart(sb, this);
            return argIndex;
        }

        public void RenderPartIndexed(StringBuilder sb, Renderer renderer, object[] args)
        {
            renderer.RenderPart(sb, this);
        }

        #endregion
    }
}
