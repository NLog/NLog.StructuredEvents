using System.Diagnostics;

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
        
        #endregion
    }
}
