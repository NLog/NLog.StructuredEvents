using System.Diagnostics;

namespace Parser
{
    [DebuggerDisplay("{Describe}")]
    public class TextPart : IPart
    {
        private readonly char? _escapeChar;
        readonly string _text;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(char c, char escapeChar)
        {
            _escapeChar = escapeChar;
            _text = c.ToString();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(string text)
        {
            this._text = text;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        private string Describe => $"Text: {_text}";

        #region Overrides of Object

        public string Print()
        {
            return _escapeChar + _text;
        }

        #endregion
    }
}
