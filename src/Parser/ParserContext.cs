using System.Collections.Generic;

namespace Parser
{
    public class ParserContext
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public ParserContext(string template)
        {
            Template = template;
        }

        public string Template { get; private set; }
        public int CharIndex { get; set; }

        public int PartIndex { get; set; }

        public char CurrentChar { get; private set; }

        public IEnumerable<char> GetNext()
        {
            for (; CharIndex < Template.Length; CharIndex++)
            {
                yield return CurrentChar = Template[CharIndex];
            }
        }




    }
}