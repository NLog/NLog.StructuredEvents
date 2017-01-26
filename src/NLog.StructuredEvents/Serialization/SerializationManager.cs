using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NLog.StructuredEvents.Serialization
{
    ///<summary>Manager serializers for customizing serialization of specific types</summary>
    public class SerializationManager
    {
        private Dictionary<Type, ISerializer> _serializers;

        private SerializationManager()
        {
            _serializers = new Dictionary<Type, ISerializer>();
        }

        /// <summary>
        /// Default serializer
        /// </summary>
        public static ISerializer DefaultSerializer = null;

        /// <summary>
        /// Instance
        /// </summary>
        public static SerializationManager Instance = new SerializationManager();
      
        /// <summary>
        /// Get the serializer for this type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ISerializer GetSerializer(Type type)
        {
            ISerializer serializer;
            if (_serializers.TryGetValue(type, out serializer))
            {
                return serializer;

            }
            return DefaultSerializer;
        }

        /// <summary>
        /// Add/update serializer for a type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        public void SaveSerializer(Type type, ISerializer serializer)
        {
            _serializers[type] = serializer;
        }

        /// <summary>
        /// Add Serializer with only a func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SaveSerializerFunc<T>(Func<T, IFormatProvider, string> func)
        {
            SaveSerializer(typeof(T), new FuncSerializer<T>(func));
        }

        /// <summary>
        /// Helper for testing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal string SerializeObject(object value)
        {
            var sb = new StringBuilder();

            SerializeObject(sb, value, CultureInfo.InvariantCulture);
            return sb.ToString();
        }

        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value"></param>
        /// <param name="formatProvider"></param>
        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            var type = value.GetType();
            var serializer = GetSerializer(type);
            if (serializer == null)
            {
                throw new RenderException($"No serializer found for type ${type}");
            }

            serializer.SerializeObject(sb, value, formatProvider);
        }
    }
}