using System;

namespace Parser
{
    public class TemplateParserException : Exception
    {
        public int Index { get; set; }

        public TemplateParserException(string message, int index) : base(message)
        {
            Index = index;
        }
    }
}