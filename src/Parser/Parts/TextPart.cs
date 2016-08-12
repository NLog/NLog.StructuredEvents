using System.Diagnostics;

namespace Parser
{
    [DebuggerDisplay("{Describe}")]
    public class TextPart : IPart
    {
        public string Text { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(string text)
        {
            Text = text;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        private string Describe => $"Text: {Text}";

        #region IPart implementation

        public string Print() => Text;
        
        #endregion
    }
}
