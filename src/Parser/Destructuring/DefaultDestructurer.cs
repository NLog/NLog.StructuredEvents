using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Parser
{
    public class DefaultDestructurer : IDestructurer
    {
        /// <summary>
        /// Cache for property infos
        /// </summary>
        private static Dictionary<Type, PropertyInfo[]> PropsCache = new Dictionary<Type, PropertyInfo[]>();


        internal string DestructureObject(object value)
        {
            var stringBuilder = new StringBuilder();
            DestructureObject(stringBuilder, value);
            return stringBuilder.ToString();
        }

        public void DestructureObject(StringBuilder sb, object value)
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
                ValueRenderer.AppendValue(sb, propValue, false, null);
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
    }
}
