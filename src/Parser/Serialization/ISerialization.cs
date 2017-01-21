using System;
using System.Text;

namespace Parser
{
    public interface ISerialization
    {
        void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}