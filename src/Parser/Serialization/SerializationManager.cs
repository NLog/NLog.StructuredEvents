using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Parser
{
    ///<summary>Manager serializer for customizing serialisation of specific types</summary>
    public class SerializationManager
    {
        private SerializationManager()
        {
            _serializers = new Dictionary<Type, ISerialisation>();
        }

        public static SerializationManager Instance = new SerializationManager();

        private Dictionary<Type, ISerialisation> _serializers;

        /// <summary>
        /// Static for static cache
        /// </summary>
        private static ISerialisation _defaultSerialisation = new DefaultSerializer();

        public ISerialisation GetSerializer(Type type)
        {
            ISerialisation serialisation;
            if (_serializers.TryGetValue(type, out serialisation))
            {
                return serialisation;

            }
            return _defaultSerialisation;
        }

        /// <summary>
        /// Add/update
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serialisation"></param>
        public void SaveSerializer(Type type, ISerialisation serialisation)
        {
            _serializers[type] = serialisation;
        }

        /// <summary>
        /// Add Serializer with only a func
        /// 
        /// Todo  overload for T: struct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SaveSerializerFunc<T>(Func<T, string> func)
            where T : class
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