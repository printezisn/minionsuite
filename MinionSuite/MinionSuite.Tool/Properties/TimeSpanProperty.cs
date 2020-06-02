using System;
using System.Collections.Generic;

namespace MinionSuite.Tool.Properties
{
    /// <summary>
    /// Represents a TimeSpan property
    /// </summary>
    public class TimeSpanProperty : IProperty
    {
        public static readonly HashSet<string> SUPPORTED_TYPES = new HashSet<string>()
        {
            "TimeSpan", "System.TimeSpan"
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
        public TimeSpanProperty(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }

        /// <summary>
        /// A default sequence value
        /// </summary>
        /// <param name="sequenceVariable">The name of the sequence variable</param>
        /// <returns>The sequence value</returns>
        public string SequenceValue(string sequenceVariable)
        {
            return $"new TimeSpan({sequenceVariable} % 24, 0, 0)";
        }
    }
}
