using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MinionSuite.Tool
{
    /// <summary>
    /// Extracts metadata from a model
    /// </summary>
    public class ModelMetadata
    {
        private readonly HashSet<string> PRIMITIVE_TYPES = new HashSet<string>()
        {
            "bool", "Boolean", "System.Boolean", "byte", "Byte", "System.Byte",
            "sbyte", "SByte", "System.SByte", "char", "Char", "System.Char",
            "decimal", "Decimal", "System.Decimal", "double", "Double", "System.Double",
            "float", "Single", "System.Single", "int", "Int32", "System.Int32",
            "uint", "UInt32", "System.UInt32", "long", "Int64", "System.Int64",
            "ulong", "UInt64", "System.UInt64", "short", "Int16", "System.Int16",
            "ushort", "UInt16", "System.UInt16", "string", "String", "System.String",
            "DateTime", "System.DateTime", "TimeSpan", "System.TimeSpan"
        };

        private readonly HashSet<string> KEY_ATTRIBUTES = new HashSet<string>()
        {
            "[Key]", "[System.ComponentModel.DataAnnotations.Key]"
        };

        /// <summary>
        /// The model name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The plural model name
        /// </summary>
        public string PluralName { get; private set; }

        /// <summary>
        /// The model namespace
        /// </summary>
        public string Namespace { get; private set; }

        /// <summary>
        /// The list of properties to care about (key: name, value: type)
        /// </summary>
        public IDictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// The name of the key property
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// The type of the key property
        /// </summary>
        public string KeyType => Properties[KeyName];

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="modelPath">The path to the model class</param>
        public ModelMetadata(string modelPath)
        {
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                throw new ArgumentException("You must enter a model path.");
            }
            if (!File.Exists(modelPath))
            {
                throw new ArgumentException($"{modelPath} doesn't exist.");
            }

            var rootNode = CSharpSyntaxTree.ParseText(File.ReadAllText(modelPath)).GetRoot();
            var namespaceNode = rootNode.DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();
            var classNode = rootNode.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();
            var propertyNodes = classNode.DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(w => PRIMITIVE_TYPES.Contains(w.Type.ToString()));

            Name = classNode.Identifier.Text;
            PluralName = Name.Pluralize();
            Namespace = namespaceNode.Name.ToString();
            Properties = propertyNodes.ToDictionary(d => d.Identifier.ValueText, d => d.Type.ToString());
            KeyName = propertyNodes
                .FirstOrDefault(f => f.AttributeLists.Any(a => KEY_ATTRIBUTES.Contains(a.ToString())))
                ?.Identifier.ValueText;
            KeyName ??= propertyNodes.FirstOrDefault(f => f.Identifier.ValueText.ToLower() == "id")
                ?.Identifier.ValueText;
        }
    }
}