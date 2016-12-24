using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Parser
{
    public class Destructurer
    {
        private static Dictionary<Type, PropertyInfo[]> PropsCache = new Dictionary<Type, PropertyInfo[]>();


        internal string DestructureObject(object value)
        {
            var stringBuilder = new StringBuilder();
            DestructureObject(value, stringBuilder);
            return stringBuilder.ToString();
        }

        public void DestructureObject(object value, StringBuilder sb)
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
                sb.Append(prop.Name);
                sb.Append(":");
                //todo escape?
                var propValue = prop.GetValue(value, null);
                AppendValue(sb, propValue, false, null);
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
                props = type.GetProperties();
                PropsCache[type] = props;
            }
           
            return props;
        }

        public static void AppendValue(StringBuilder sb, object value, bool legacy, string format)
        {

            string stringValue;
            // Shortcut common case. It is important to do this before IEnumerable, as string _is_ IEnumerable
            if ((stringValue = value as string) != null)
            {
                AppendValueAsString(sb, stringValue, legacy, format);
                return;
            }


            IEnumerable collection;
            if (!legacy && (collection = value as IEnumerable) != null)
            {
                bool separator = false;
                foreach (var item in collection)
                {
                    if (separator) sb.Append(", ");
                    AppendValue(sb, item, false, format);
                    separator = true;
                }
                return;
            }

            IFormattable formattable;
            if (format != null && (formattable = value as IFormattable) != null)
            {
                sb.Append(formattable.ToString(format, CultureInfo.CurrentCulture));
            }

            else if (value is char)
            {
                if (legacy || format == "l")
                    sb.Append((char)value);
                else
                    sb.Append('"').Append((char)value).Append('"');
            }
            else
            {
                sb.Append(value.ToString());
            }
        }

        private static void AppendValueAsString(StringBuilder sb, string stringValue, bool legacy, string format)
        {
            if (legacy || format == "l")
                sb.Append(stringValue);
            else
                sb.Append('"').Append(stringValue).Append('"');
        }
    }
}
