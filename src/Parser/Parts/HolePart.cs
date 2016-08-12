using System;
using System.Diagnostics;

namespace Parser
{

    [DebuggerDisplay("{Describe}")]
    public class HolePart : IPart
    {
        private string _name;
        private int _holeIndex;
        private string _format;

        private HoleType _type;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public HolePart(string name, HoleType type, int holeIndex, string format)
        {
            this._name = name;
            _holeIndex = holeIndex;
            _format = format;
            _type = type;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        private string Describe => $"Hole: Name: {_name}, HoleIndex: {_holeIndex}, Format: {_format}, Type: {_type}";

        private string GetNameAndFormat()
        {
            if (_format != null)
            {
                return _name + ":" + _format;
            }
            return _name;
        }

        public string Print()
        {
            var nameAndFormat = GetNameAndFormat();
            switch (_type)
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