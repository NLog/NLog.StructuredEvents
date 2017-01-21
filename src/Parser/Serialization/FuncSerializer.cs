using System;
using System.Text;

namespace Parser
{
    public class FuncSerializer<T> : ISerialization
    {
        private Func<T, string> _func;

        public FuncSerializer(Func<T, string> func)
        {
            _func = func;
        }

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            if (value is T)
            {
                sb.Append(_func((T)value));
            }
        }
    }
}