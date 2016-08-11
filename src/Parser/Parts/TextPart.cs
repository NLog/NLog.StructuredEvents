using System.Diagnostics;

namespace Parser
{
    [DebuggerDisplay("{Describe}")]
    public class TextPart : IPart
    {
        readonly string _text;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(string text)
        {
            this._text = text;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        private string Describe => $"Text: {_text}";

        #region IPart implementation

        public string Print() => _text;
        
        #endregion
    }
}
