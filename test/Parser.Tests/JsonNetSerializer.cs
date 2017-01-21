using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Parser.Tests
{
    /// <summary>
    /// Serializer using the great JSON.NET
    /// </summary>
    public class JsonNetSerializer : ISerializer
    {

        public static ISerializer Instance => new JsonNetSerializer();

        #region Implementation of ISerialization

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            sb.AppendFormat(formatProvider, "{0}", JsonConvert.SerializeObject(value));
        }

        #endregion
    }
}
