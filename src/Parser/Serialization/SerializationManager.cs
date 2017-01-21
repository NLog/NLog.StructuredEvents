using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Parser
{
    ///<summary>Manager serializers for customizing serialization of specific types</summary>
    public class SerializationManager
    {
        private Dictionary<Type, ISerialization> _serializers;

        private SerializationManager()
        {
            _serializers = new Dictionary<Type, ISerialization>();
        }

        /// <summary>
        /// Default serializer
        /// </summary>
        public static ISerialization DefaultSerialization = null;

        /// <summary>
        /// Instance
        /// </summary>
        public static SerializationManager Instance = new SerializationManager();
      
        /// <summary>
        /// Get the serializer for this type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ISerialization GetSerializer(Type type)
        {
            ISerialization serialization;
            if (_serializers.TryGetValue(type, out serialization))
            {
                return serialization;

            }
            return DefaultSerialization;
        }

        /// <summary>
        /// Add/update serializer for a type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serialization"></param>
        public void SaveSerializer(Type type, ISerialization serialization)
        {
            _serializers[type] = serialization;
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
                throw new Exception("No serializer found");
            }

            serializer.SerializeObject(sb, value, formatProvider);
        }
    }
}