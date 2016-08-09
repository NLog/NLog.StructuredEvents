using System.Diagnostics;

namespace Parser
{
    [DebuggerDisplay("{Describe}")]
    public class TextPart : IPart
    {
        readonly string _text;

        /// <summary>
        /// Escpaped text
        /// </summary>
        private bool _escaped;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(char c, bool escaped)
        {
            _escaped = escaped;
            _text = c.ToString();
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TextPart(string text, bool escaped)
        {
            this._text = text;
            _escaped = escaped;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        private string Describe => $"Text: {_text}";

        #region Overrides of Object

        public string Print()
        {
            if (_escaped)
            {
                return "{" + _text + "}";
            }

            return _text;
        }

        #endregion
    }
}
