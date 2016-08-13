using System;
using System.Diagnostics;
using System.Text;

namespace Parser
{

    [DebuggerDisplay("Hole: Name: {Name}, HoleIndex: {HoleIndex}, Format: {Format}, Type: {HoleType}")]
    public class HolePart : IPart
    {
        public string Name { get; }
        public int HoleIndex { get; }
        public string Format { get; }
        public int? Aligment { get; }
        public HoleType HoleType { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public HolePart(string name, HoleType type, int holeIndex, string format, int? aligment)
        {
            Name = name;
            HoleIndex = holeIndex;
            Format = format;
            HoleType = type;
            Aligment = aligment;
        }

        private string GetNameAndFormat()
        {
            if (Format != null)
            {
                return Name + ":" + Format;
            }
            return Name;
        }

        public string Print()
        {
            var nameAndFormat = GetNameAndFormat();
            switch (HoleType)
            {
                case HoleType.Destructuring:
                    return "{@" + nameAndFormat + "}";
                case HoleType.Stringification:
                    return "{$" + nameAndFormat + "}";
                default:
                    //else HoleType.Numeric / HoleType.Text:
                    return "{" + nameAndFormat + "}";
            }
        }



        public void RenderPartIndexed(StringBuilder sb, Renderer renderer, object[] args)
        {
            //no qoutes to be backwardscomp.
            renderer.RenderPart(sb, this, this.HoleIndex, false, args);
        }

        public int RenderPart(StringBuilder sb, Renderer renderer, int argIndex, object[] args)
        {
            renderer.RenderPart(sb, this, argIndex, true, args);
            return argIndex + 1;
        }
    }
}