namespace NLog.StructuredEvents.Parts
{
    /// <summary>
    /// The type of the captured hole
    /// </summary>
    public enum CaptureType : byte
    {
        /// <summary>
        /// normal {x}
        /// </summary>
        Normal,
        /// <summary>
        ///  Serialize operator {@x} (aka destructure)
        /// </summary>
        Serialize,
        /// <summary>
        /// stringification operator {$x} 
        /// </summary>
        Stringify,
    }
}