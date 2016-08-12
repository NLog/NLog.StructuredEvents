namespace Parser
{
    public enum HoleType
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