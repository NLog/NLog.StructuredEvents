using System;
using System.Text;

namespace Parser
{
    public class FuncDestructurer<T> : IDestructurer
        where T : class //todo add also class for struct
    {
        #region Implementation of IDestructurer

        private Func<T, string> _func;

        public FuncDestructurer(Func<T, string> func)
        {
            _func = func;
        }

        public void DestructureObject(StringBuilder sb, object value)
        {
            var o = value as T;
            if (o != null)
            {
                sb.Append(_func(o));
            }
        }

        #endregion
    }
}