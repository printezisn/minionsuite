using System;
namespace MinionSuite.Tool.Properties
{
    /// <summary>
    /// Factory that returns the appropriate property
    /// </summary>
    public static class PropertyFactory
    {
        /// <summary>
        /// Returns the appropriate property based on the type
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="typeName">The property type</param>
        /// <returns>The generated prooperty</returns>
        public static IProperty GetProperty(string name, string typeName)
        {
            if (BooleanProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new BooleanProperty(name, typeName);
            }
            if (CharProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new CharProperty(name, typeName);
            }
            if (DateTimeProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new DateTimeProperty(name, typeName);
            }
            if (DoubleProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new DoubleProperty(name, typeName);
            }
            if (IntegerProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new IntegerProperty(name, typeName);
            }
            if (StringProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new StringProperty(name, typeName);
            }
            if (TimeSpanProperty.SUPPORTED_TYPES.Contains(typeName))
            {
                return new TimeSpanProperty(name, typeName);
            }

            return null;
        }
    }
}
