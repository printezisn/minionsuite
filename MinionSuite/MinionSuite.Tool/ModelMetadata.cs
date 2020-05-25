using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MinionSuite.Tool.Properties;

namespace MinionSuite.Tool
{
    /// <summary>
    /// Extracts metadata from a model
    /// </summary>
    public class ModelMetadata
    {
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
        /// The list of properties to care about (key: name, value: property)
        /// </summary>
        public IDictionary<string, IProperty> Properties { get; private set; }

        /// <summary>
        /// The name of the key property
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// The key property
        /// </summary>
        public IProperty KeyProperty => Properties[KeyName];

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
            var propertyNodes = classNode.DescendantNodes().OfType<PropertyDeclarationSyntax>();

            Name = classNode.Identifier.Text;
            PluralName = Name.Pluralize();
            Namespace = namespaceNode.Name.ToString();
            Properties = propertyNodes
                .Select(s => PropertyFactory.GetProperty(s.Identifier.ValueText, s.Type.ToString()))
                .Where(w => w != null)
                .ToDictionary(d => d.Name, d => d);
            KeyName = propertyNodes
                .FirstOrDefault(f => f.AttributeLists.Any(a => KEY_ATTRIBUTES.Contains(a.ToString())))
                ?.Identifier.ValueText;
            KeyName ??= propertyNodes.FirstOrDefault(f => f.Identifier.ValueText.ToLower() == "id")
                ?.Identifier.ValueText;
        }
    }
}