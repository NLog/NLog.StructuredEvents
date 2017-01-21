using System;
using System.Text;

namespace Parser
{
    public class FuncSerializer<T> : ISerialization
        where T : class //todo add also class for struct
    {
        private Func<T, string> _func;

        public FuncSerializer(Func<T, string> func)
        {
            _func = func;
        }

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            var o = value as T;
            if (o != null)
            {
                sb.Append(_func(o));
            }
        }
    }
}