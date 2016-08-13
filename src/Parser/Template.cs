using System.Collections.Generic;
using System.Text;

namespace Parser
{
  public class Template
  {
    // The original template string.
    // This is the key passed to structured targets.     
    public string Value { get; }

    // This is the list of literal parts, useful for string rendering.
    // It indicates the number of characters from the original string to print,
    // then there's a hole with how many chars to skip.
    // Examples:
    // |Hello |{firstName}| |{lastName}|!|
    // |6     |11         |1|10        |1|0
    //   |{x}| * 2 = |{2x}
    // |0|3  |7      |4
    // The problem is escaped braces. They are represented by a skip = 0,
    // which is interpreted as "move one char forward, no hole".
    // |Escaped }|}| is fun.|
    // |9        |0|8       |0
    public List<Literal> Literals { get; } = new List<Literal>();

    // This is the list of holes. It's used both to fill the string rendering
    // and to send values along the template to structured targets.
    public List<Hole> Holes { get; } = new List<Hole>();

    // Indicates whether the template should be interpreted as positional 
    // (all holes are numbers) or named.
    public bool IsPositional { get; set; } = true;

    public Template(string template)
    {
      Value = template;
    }

    // This is for testing only: recreates .Value from the parsed data
    public string Rebuild()
    {
      var sb = new StringBuilder(Value.Length);
      int pos = 0;
      int h = 0;
      foreach (var literal in Literals)
      {
        sb.Append(Value, pos, literal.Print);
        pos += literal.Print;
        if (literal.Skip == 0)
        {
          // 0 means escaping or end of string without hole.
          if (pos < Value.Length) 
            sb.Append(Value[pos++]);
        }
        else
        {
          pos += literal.Skip;
          var hole = Holes[h++];

          if (hole.CaptureType == HoleType.Normal)
            sb.Append('{');
          else if (hole.CaptureType == HoleType.Destructuring)
            sb.Append("{@");
          else  // hole.CaptureType == HoleType.Stringification
            sb.Append("{$");

          sb.Append(hole.Name);

          if (hole.Alignment != 0)
            sb.Append(',').Append(hole.Alignment);
          
          if (hole.Format != null)
            sb.Append(':').Append(hole.Format);
          
          sb.Append('}');
        }
      }
      return sb.ToString();
    }
  }

  public struct Literal
  {
    // Number of characters to print
    public ushort Print;
    // Number of characters to skip. 0 is a special value that mean: 1 escaped char, no hole.
    public ushort Skip; 
  }

  public struct Hole
  {
    // Sent to structured loggers
    public string Name;
    // Used to render strings
    public string Format;    
    public HoleType CaptureType; 
    // Used to get the correct parameter when rendering positional templates.
    public byte Index;
    // Used to render strings. 0 means no alignment.
    public short Alignment;
  }
}