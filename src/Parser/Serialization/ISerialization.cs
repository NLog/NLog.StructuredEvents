﻿using System;
using System.Text;

namespace Parser
{
   ///<summary>Serializer</summary>
    public interface ISerialization
    {
        /// <summary>Serialize an object</summary>
        /// <param name="sb">Add serialized value to this builder</param>
        /// <param name="value">Value to be serialized</param>
        /// <param name="formatProvider">format</param>
        void SerializeObject(StringBuilder sb, object value, IFormatProvider formatProvider);
    }
}