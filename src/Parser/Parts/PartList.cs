using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public class PartList : List<IPart>
    {
        public bool? IsPositional { get; set; }
        
        public PartList()
        { }

        public string Print()
        {
            return this.Aggregate("", (current, part) => current + part.Print());
        }
    }
}
