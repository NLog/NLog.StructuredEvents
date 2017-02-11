using System;

namespace NLog.StructuredEvents
{
    /// <summary>
    /// Error when parsing a template.
    /// </summary>
    public class TemplateParserException : Exception
    {
        /// <summary>
        /// Current index when the error occurred.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// New exception
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="index">Current index when the error occurred.</param>
        public TemplateParserException(string message, int index) : base(message)
        {
            Index = index;
        }
    }
}