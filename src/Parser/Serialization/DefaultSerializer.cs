using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Parser
{

    ///<summary>TODO combine with NLog's Json (injected) serializer?</summary>
    public class DefaultSerializer : ISerialisation
    {
        /// <summary>
        /// Cache for property infos
        /// </summary>
        private static Dictionary<Type, PropertyInfo[]> PropsCache = new Dictionary<Type, PropertyInfo[]>();


        internal string SerializeObject(object value)
        {
            var stringBuilder = new StringBuilder();
            SerializeObject(stringBuilder, value, CultureInfo.InvariantCulture);
            return stringBuilder.ToString();
        }

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            var props = GetProps(value);

            sb.Append('{');

            var isFirst = true;

            foreach (var prop in props)
            {
                if (!isFirst)
                {
                    sb.Append(", ");
                }
                isFirst = false;
                //todo escape name? (e.g. spaces)
                sb.Append(prop.Name);
                sb.Append(":");
                //todo escape value? (e.g quotes)
                var propValue = prop.GetValue(value, null);
//todo nest objects, be warn of infinite loops.
                ValueRenderer.AppendValue(sb, propValue, false, null, formatProvider);
            }
            sb.Append('}');

        }

        /// <summary>
        /// Get properties, cached for a type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProps(object value)
        {
            var type = value.GetType();
            PropertyInfo[] props;
            if (!PropsCache.TryGetValue(type, out props))
            {
#if NETSTANDARD
                props = type.GetRuntimeProperties().ToArray();
#else
                props = type.GetProperties();
#endif
                PropsCache[type] = props;
            }
           
            return props;
        }
    }
}
