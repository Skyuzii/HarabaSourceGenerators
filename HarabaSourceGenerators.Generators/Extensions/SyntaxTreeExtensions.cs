using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HarabaSourceGenerators.Generators.Extensions
{
    public static class SyntaxTreeExtensions
    {
        public static bool ContainsClassAttribute(this ClassDeclarationSyntax classDeclaration, string attributeName)
        {
            return classDeclaration.AttributeLists.Any(x => x.Attributes.Any(z => z.Name.ToString() == attributeName));
        }

        public static bool ContainsFieldAttribute(this ClassDeclarationSyntax classDeclaration, string attributeName)
        {
            return classDeclaration.Members.OfType<FieldDeclarationSyntax>()
                        .Any(e => e.AttributeLists
                            .Any(z => z.Attributes
                                .Any(y => y.Name.ToString() == attributeName)));
        }
    }
}
