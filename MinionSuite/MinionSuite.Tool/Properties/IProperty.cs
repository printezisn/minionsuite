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
        /// A default value
        /// </summary>
        /// <param name="index">An index to make the value unique</param>
        /// <returns>The default value</returns>
        string DefaultValue(int index);
    }
}
