namespace NLog.StructuredEvents.Parts
{
    /// <summary>
    /// A hole that will be replaced with a value
    /// </summary>
    public struct Hole
    {
        /// <summary>Parameter name sent to structured loggers.</summary>
        /// <remarks>This is everything between "{" and the first of ",:}". 
        /// Including surrounding spaces and names that are numbers.</remarks>
        public string Name;
        /// <summary>Format to render the parameter.</summary>
        /// <remarks>This is everything between ":" and the first unescaped "}"</remarks>
        public string Format;
        /// <summary>
        /// Type
        /// </summary>
        public CaptureType CaptureType;
        /// <summary>When the template is positional, this is the parsed name of this parameter.</summary>
        /// <remarks>For named templates, the value of Index is undefined.</remarks>
        public byte Index;
        /// <summary>Alignment to render the parameter, by default 0.</summary>
        /// <remarks>This is the parsed value between "," and the first of ":}"</remarks>
        public short Alignment;
    }
}