using System;
using System.Diagnostics;

namespace Parser
{

    [DebuggerDisplay("{Describe}")]
    public class HolePart : IPart
    {
        public string Name { get; }
        public int HoleIndex { get; }
        public string Format { get; }
        public string Aligment { get; }
        public HoleType HoleType { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public HolePart(string name, HoleType type, int holeIndex, string format, string aligment)
        {
            Name = name;
            HoleIndex = holeIndex;
            Format = format;
            HoleType = type;
            Aligment = aligment;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        private string Describe => $"Hole: Name: {Name}, HoleIndex: {HoleIndex}, Format: {Format}, Type: {HoleType}";

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
                case HoleType.Numeric:
                case HoleType.Text:
                    return "{" + nameAndFormat + "}";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}