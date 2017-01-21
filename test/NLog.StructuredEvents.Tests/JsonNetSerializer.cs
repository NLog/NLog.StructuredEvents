using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NLog.StructuredEvents.Serialization;

namespace NLog.StructuredEvents.Tests
{
    /// <summary>
    /// Serializer using the great JSON.NET
    /// </summary>
    public class JsonNetSerializer : ISerializer
    {
        private readonly bool _singleQuote;
        private readonly bool _alwaysQuote;
        private JsonSerializer _serializer;

        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public JsonNetSerializer(bool singleQuote, bool alwaysQuote)
        {
            _singleQuote = singleQuote;
            _alwaysQuote = alwaysQuote;
            _serializer = new JsonSerializer();
        }


        public static ISerializer Instance => new JsonNetSerializer(false, false);

        #region Implementation of ISerialization

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            var stringWriter = new StringWriter(sb, formatProvider);
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = !_alwaysQuote;
                if (_singleQuote)
                {
                    writer.QuoteChar = '\'';
                }
                _serializer.Serialize(writer, value);
            }
        }

        #endregion
    }
}
