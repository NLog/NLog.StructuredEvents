namespace Parser
{
    public struct Literal
    {
        /// <summary>Number of characters from the original template to copy at the current position.</summary>
        /// <remarks>This can be 0 when the template starts with a hole or when there are multiple consecutive holes.</remarks>
        public ushort Print;
        /// <summary>Number of characters to skip in the original template at the current position.</summary>
        /// <remarks>0 is a special value that mean: 1 escaped char, no hole. It can also happen last when the template ends with a literal.</summary>
        public ushort Skip;
    }
}