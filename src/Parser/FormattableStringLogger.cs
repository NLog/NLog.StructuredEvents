using System;
using System.Collections.Generic;
using System.Linq;

namespace Parser
{
    public class FormattableStringLogger
    {


        public static string Log(FormattableString formattable)
        {
            var extracter = new ExtractObjectsFormatter();
            var message = formattable.ToString(extracter);
            var parts = extracter.args;
            return message;
        }

    }

    /// <summary>
    /// todo in real implementation, the implementation of the two interfaces is prob splitted.
    /// </summary>
    class ExtractObjectsFormatter : IFormatProvider, ICustomFormatter
    {

        public List<object> args;


        public ExtractObjectsFormatter()
        {
            args = new List<object>();
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            return null;
        }


        #region Implementation of ICustomFormatter

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {


            args.Add(arg);
            

            return format != null ? string.Format(format, arg) : arg?.ToString();
        }

        #endregion
    }
}
