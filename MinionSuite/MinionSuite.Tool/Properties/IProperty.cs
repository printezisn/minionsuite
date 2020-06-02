using System;
namespace MinionSuite.Tool.Properties
{
    /// <summary>
    /// Represents a property
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// The property name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The property type
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// A default sequence value
        /// </summary>
        /// <param name="sequenceVariable">The name of the sequence variable</param>
        /// <returns>The sequence value</returns>
        string SequenceValue(string sequenceVariable);
    }
}
