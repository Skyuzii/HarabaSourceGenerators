using HarabaSourceGenerators.Common.Attributes;
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
            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var targetTypes = syntaxTree.GetRoot().DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .Where(x => x.Members.OfType<FieldDeclarationSyntax>()
                        .Any(e => e.AttributeLists
                            .Any(z => z.Attributes
                                .Any(y => y.Name.ToString() == nameof(InjectAttribute).Replace("Attribute", string.Empty)))))
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
using Microsoft.Extensions.DependencyInjection;

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

            foreach (var field in targetType.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(x => x.GetAttributes().Any(y => y.AttributeClass.Name == nameof(InjectAttribute))))
            {
                parameters.AppendLine($"{field.Name} = serviceProvider.GetRequiredService<{field.Type}>();");
            }

            return $@"public {targetType.Name}(IServiceProvider serviceProvider)
                      {{
                          {parameters}
                      }}";
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
