using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Parser
{
    ///<summary>Manager serializer for customizing serialization of specific types</summary>
    public class SerializationManager
    {
        private SerializationManager()
        {
            _serializers = new Dictionary<Type, ISerialization>();
        }

        public static SerializationManager Instance = new SerializationManager();

        private Dictionary<Type, ISerialization> _serializers;

        /// <summary>
        /// Static for static cache
        /// </summary>
        private static ISerialization _defaultSerialization = new DefaultSerializer();

        public ISerialization GetSerializer(Type type)
        {
            ISerialization serialization;
            if (_serializers.TryGetValue(type, out serialization))
            {
                return serialization;

            }
            return _defaultSerialization;
        }

        /// <summary>
        /// Add/update
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
        /// TODO docs
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

        public void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            var type = value.GetType();
            var Serializer = GetSerializer(type);
            Serializer.SerializeObject(sb, value, formatProvider);
        }
    }
}