namespace Parser
{
    public struct Hole
    {
        // Sent to structured loggers
        public string Name;
        // Used to render strings
        public string Format;
        public CaptureType CaptureType;
        // Used to get the correct parameter when rendering positional templates.
        public byte Index;
        // Used to render strings. 0 means no alignment.
        public short Alignment;
    }
}