using System;
using System.Text;

namespace MinionSuite.Tool.Extensions
{
    /// <summary>
    /// Contains extensions for the StringBuilder class
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a line along with nested indentation
        /// </summary>
        /// <param name="builder">The StringBuilder object</param>
        /// <param name="depth">The nested depth</param>
        /// <param name="line">The line to append</param>
        /// <returns>The StringBuilder object</returns>
        public static StringBuilder AppendNestedLine(this StringBuilder builder, int depth, string line)
        {
            for (int i = 0; i < depth * 4; i++)
            {
                builder.Append(" ");
            }

            return builder.AppendLine(line);
        }
    }
}
