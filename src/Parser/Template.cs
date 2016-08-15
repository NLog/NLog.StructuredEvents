using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Parser
{
  public class Template
  {
    /// <summary>The original template string.</summary>
    /// <remarks>This is the key passed to structured targets.</remarks>     
    public string Value { get; }

    /// <summary>The list of literal parts, useful for string rendering.
    /// It indicates the number of characters from the original string to print,
    /// then there's a hole with how many chars to skip.</summary>
    /// <example>
    /// "Hello {firstName} {lastName}!"
    /// -------------------------------------
    /// ║P     |S          ║P|S         ║P|S║
    /// ║6     |11         ║1|10        ║1|0║
    /// ║Hello |{firstName}║ |{lastName}║!║
    /// 
    /// "{x} * 2 = {2x}"
    /// --------------------
    /// ║P|S  ║P      |S   ║
    /// ║0|3  ║7      |4   ║
    ///   ║{x}║ * 2 = |{2x}║
    /// 
    /// The tricky part is escaped braces. They are represented by a skip = 0,
    /// which is interpreted as "move one char forward, no hole".
    /// 
    /// "Escaped }} is fun."
    /// ----------------------
    /// ║P        |S║P       |S║
    /// ║9        |0║8       |0║
    /// ║Escaped }|}║ is fun.|║
    /// </example>
    public Literal[] Literals { get; }

    /// <summary> This list of holes. It's used both to fill the string rendering
    /// and to send values along the template to structured targets.</summary>
    public Hole[] Holes { get; }

    /// <summary>Indicates whether the template should be interpreted as positional 
    /// (all holes are numbers) or named.</summary>
    public bool IsPositional { get; }

    public Template(string template, bool isPositional, List<Literal> literals, List<Hole> holes)
    {
      Value = template;
      IsPositional = isPositional;
      // Using arrays is important! It's the only CLR type that will give us a no-copy access to 
      // the structs contained in the array.
      Literals = literals.ToArray();
      Holes = holes.ToArray();
    }

    public string Render(object[] parameters)
    {
        var sb = new StringBuilder(Value.Length + 64 * Holes.Length);
        int pos = 0;
        int h = 0;
        foreach (var literal in Literals)
        {
            sb.Append(Value, pos, literal.Print);
            pos += literal.Print;
            if (literal.Skip == 0) 
                pos++;
            else
            {
                pos += literal.Skip;
                if (IsPositional)
                    RenderHolePositional(sb, ref Holes[h++], parameters);
                else
                    RenderHole(sb, ref Holes[h], parameters[h++]);
            }
        }
        return sb.ToString();
    }

    private static void RenderHolePositional(StringBuilder sb, ref Hole hole, object[] parameters)
        => RenderHole(sb, ref hole, parameters[hole.Index], true);

    private static void RenderHole(StringBuilder sb, ref Hole hole, object value, bool legacy = false)
    {
        // TODO: handle value == null
        
        // TODO: destructuring {@x}
        
        if (hole.CaptureType == CaptureType.Stringification)
        {
            // TODO: we don't need to support format and alignment here?
            sb.Append('"').Append(value.ToString()).Append('"');
            return;
        }

        // Shortcut common case. It is important to do this before IEnumerable, as string _is_ IEnumerable
        if (value is string)
        {
            AppendValue(sb, ref hole, value, legacy);
            return;
        }

        IEnumerable collection;
        if (!legacy && (collection = value as IEnumerable) != null)
        {
            int pos = sb.Length;
            foreach (var item in collection) {
                AppendValue(sb, ref hole, item, false);
                sb.Append(", ");
            }
            if (sb.Length > pos) sb.Length -= 2; // Remove trailing ", "
            return;
        }

        AppendValue(sb, ref hole, value, legacy);
    }

    private static void AppendValue(StringBuilder sb, ref Hole hole, object value, bool legacy)
    {
        // TODO: value can be null again (from IEnumerable)
        IFormattable formattable;
        if (hole.Format != null && (formattable = value as IFormattable) != null)
        {
            sb.Append(formattable.ToString(hole.Format, CultureInfo.CurrentCulture));
        }
        else if (value is string)
        {
            if (legacy || hole.Format == "l")
                sb.Append((string)value);
            else
                sb.Append('"').Append((string)value).Append('"');
        }
        else if (value is char)
        {
            if (legacy || hole.Format == "l")
                sb.Append((char)value);
            else
                sb.Append('"').Append((char)value).Append('"');
        }
        else
        {
            sb.Append(value.ToString());
        }
    }

    /// <summary>This is for testing only: recreates <see cref=Value /> from the parsed data.</summary>
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
          RebuildHole(sb, ref Holes[h++]);
        }
      }
      return sb.ToString();
    }

    private static void RebuildHole(StringBuilder sb, ref Hole hole)
    {
        if (hole.CaptureType == CaptureType.Normal)
            sb.Append('{');
        else if (hole.CaptureType == CaptureType.Destructuring)
            sb.Append("{@");
        else  // hole.CaptureType == CaptureType.Stringification
            sb.Append("{$");

        sb.Append(hole.Name);

        if (hole.Alignment != 0)
            sb.Append(',').Append(hole.Alignment);
          
        if (hole.Format != null)
            sb.Append(':').Append(hole.Format);
          
        sb.Append('}');
    }
  }
}