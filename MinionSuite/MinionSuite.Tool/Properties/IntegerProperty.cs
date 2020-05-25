using System;
using System.Collections.Generic;

namespace MinionSuite.Tool.Properties
{
    /// <summary>
    /// Represents an integer property
    /// </summary>
    public class IntegerProperty : IProperty
    {
        public static readonly HashSet<string> SUPPORTED_TYPES = new HashSet<string>()
        {
            "short", "Int16", "System.Int16", "int", "Int32", "System.Int32",
            "long", "Int64", "System.Int64", "byte", "Byte", "System.Byte",
            "ushort", "UInt16", "System.UInt16", "uint", "UInt32", "System.UInt32",
            "ulong", "UInt64", "System.UInt64", "sbyte", "SByte", "System.SByte"
        };

        /// <summary>
        /// The property name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The property type
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="typeName">The property type</param>
        public IntegerProperty(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }
    }
}
