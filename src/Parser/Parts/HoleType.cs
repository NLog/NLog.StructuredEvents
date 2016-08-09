namespace Parser
{
    public enum HoleType
    {
        /// <summary>
        ///  destructuring operator (@) 
        /// </summary>
        Destructuring,
        /// <summary>
        /// stringification operator ($) 
        /// </summary>
        Stringification,
        /// <summary>
        /// {0} etc
        /// </summary>
        Numeric,
        /// <summary>
        /// {car} etc
        /// </summary>
        Text

    }
}