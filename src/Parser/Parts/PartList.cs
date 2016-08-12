using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public class PartList : List<IPart>
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.List`1" /> class that is empty and has the default initial capacity.</summary>
        public PartList()
        {
        }

        public string Print()
        {
            return this.Aggregate("", (current, part) => current + part.Print());
        }
    }
}
