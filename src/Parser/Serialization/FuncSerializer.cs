using System;
using System.Text;

namespace Parser
{
    public class FuncSerializer<T> : ISerialization
    {
        private Func<T, IFormatProvider, string> _func;

        public FuncSerializer(Func<T, IFormatProvider, string> func)
        {
            _func = func;
        }

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            if (value is T)
            {
                sb.Append(_func((T)value, formatProvider));
            }
        }
    }
}