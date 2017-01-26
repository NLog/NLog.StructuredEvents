using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.StructuredEvents.Serialization;

namespace NLog.StructuredEvents.Tests
{
    public static class TestExt
    {
        /// <summary>
        /// Helper for testing
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string SerializeObject(this SerializationManager manager, object value)
        {
            var sb = new StringBuilder();

            manager.SerializeObject(sb, value, CultureInfo.InvariantCulture);
            return sb.ToString();
        }
    }
}
