using System;
using System.Text;

namespace Parser
{
    public interface ISerialisation
    {
        void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}