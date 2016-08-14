namespace Parser
{
    public struct Literal
    {
        // Number of characters to print
        public ushort Print;
        // Number of characters to skip. 0 is a special value that mean: 1 escaped char, no hole.
        public ushort Skip;
    }
}