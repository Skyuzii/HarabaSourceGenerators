﻿using HarabaSourceGenerators.Common.Attributes;
using HarabaSourceGenerators.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HarabaSourceGenerators.Generators
{
    [Generator]
    public class InjectSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;
            var attributeName = nameof(InjectAttribute).Replace("Attribute", string.Empty);
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var targetTypes = syntaxTree.GetRoot().DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(x => x.ContainsClassAttribute(attributeName) || x.ContainsFieldAttribute(attributeName))
                    .Select(x => semanticModel.GetDeclaredSymbol(x))
                    .OfType<ITypeSymbol>();

                foreach (var targetType in targetTypes)
                {
                    string source = GenerateInjects(targetType);
                    context.AddSource($"{targetType.Name}.Constructor.cs", SourceText.From(source, Encoding.UTF8));
                }
            }
        }

        private string GenerateInjects(ITypeSymbol targetType)
        {
            return $@" 
using System;

namespace {targetType.ContainingNamespace}
{{
    public partial class {targetType.Name}
    {{
        {GenerateConstructor(targetType)}
    }}
}}";
        }

        private string GenerateConstructor(ITypeSymbol targetType)
        {
            var parameters = new StringBuilder();
            var fieldsInitializing = new StringBuilder();
            var fields = targetType.GetAttributes().Any(x => x.AttributeClass.Name == nameof(InjectAttribute)) 
                            ? targetType.GetMembers()
                                .OfType<IFieldSymbol>()
                                .Where(x => !x.IsStatic && x.IsReadOnly && !x.GetAttributes().Any(y => y.AttributeClass.Name == nameof(InjectIgnoreAttribute)))
                            : targetType.GetMembers()
                                .OfType<IFieldSymbol>()
                                .Where(x => x.GetAttributes().Any(y => y.AttributeClass.Name == nameof(InjectAttribute)));

            var orderedFields = fields.OrderBy(x => x.GetAttributes()
                                                     .FirstOrDefault(e => e.AttributeClass.Name == nameof(InjectAttribute))
                                                     ?.ConstructorArguments.FirstOrDefault().Value ?? default(int)).ToList();
            foreach (var field in orderedFields)
            {
                var parameterName = field.Name.TrimStart('_');
                parameters.Append($"{field.Type} {parameterName},");
                fieldsInitializing.AppendLine($"this.{field.Name} = {parameterName};");
            }

            return $@"public {targetType.Name}({parameters.ToString().TrimEnd(',')})
                      {{
                          {fieldsInitializing}
                      }}";
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
