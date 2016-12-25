using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Parser
{
    public class DestructorManager
    {
        private DestructorManager()
        {
            _destructurers = new Dictionary<Type, IDestructurer>();
        }

        public static DestructorManager Instance = new DestructorManager();

        private Dictionary<Type, IDestructurer> _destructurers;

        /// <summary>
        /// Static for static cache
        /// </summary>
        private static IDestructurer DefaultDestructurer = new DefaultDestructurer();

        public IDestructurer GetDestructurer(Type type)
        {
            IDestructurer destructurer;
            if (_destructurers.TryGetValue(type, out destructurer))
            {
                return destructurer;

            }
            return DefaultDestructurer;
        }

        /// <summary>
        /// Add/update
        /// </summary>
        /// <param name="type"></param>
        /// <param name="destructurer"></param>
        public void SaveDestructur(Type type, IDestructurer destructurer)
        {
            _destructurers[type] = destructurer;
        }

        /// <summary>
        /// Add destructur with only a func
        /// 
        /// Todo struct overload
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        public void SaveDestructurFunc<T>(Func<T, string> func)
            where T : class
        {
            SaveDestructur(typeof(T), new FuncDestructurer<T>(func));
        }

        /// <summary>
        /// TODO docs
        /// Helper for testing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal string DestructureObject(object value)
        {
            var sb = new StringBuilder();

            DestructureObject(sb, value, CultureInfo.InvariantCulture);
            return sb.ToString();
        }

        public void DestructureObject(StringBuilder sb, object value, IFormatProvider formatProvider)
        {
            var type = value.GetType();
            var destructurer = GetDestructurer(type);
            destructurer.DestructureObject(sb, value, formatProvider);
        }
    }
}