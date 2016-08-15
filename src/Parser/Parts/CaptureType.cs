namespace Parser
{
    public enum CaptureType : byte
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