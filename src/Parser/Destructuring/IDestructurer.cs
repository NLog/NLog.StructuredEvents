using System;
using System.Text;

namespace Parser
{
    public interface IDestructurer
    {
        void DestructureObject(StringBuilder sb, object value);
    }
}