using System;
using System.Text;

namespace NLog.StructuredEvents.Serialization
{
    /// <summary>
    /// Serialize a type with a (lamdba) function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FuncSerializer<T> : ISerializer
    {
        private Func<T, IFormatProvider, string> _func;

        public FuncSerializer(Func<T, IFormatProvider, string> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            _func = func;
        }

        /// <inheritdoc />
        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            if (value is T)
            {
                sb.Append(_func((T)value, formatProvider));
            }
        }
    }
}