namespace Parser
{
    public enum HoleType : byte
    {
        /// <summary>
        /// normal {x}
        /// </summary>
        Normal,
        /// <summary>
        ///  destructuring operator {@x} 
        /// </summary>
        Destructuring,
        /// <summary>
        /// stringification operator {$x} 
        /// </summary>
        Stringification,
    }
}