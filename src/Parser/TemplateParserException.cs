using System;

namespace Parser
{
    public class TemplateParserException : Exception
    {
        public TemplateParserException(string message) : base(message)
        { }
    }
}