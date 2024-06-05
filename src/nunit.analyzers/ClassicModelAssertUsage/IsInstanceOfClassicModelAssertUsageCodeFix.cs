using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Analyzers.Constants;

namespace NUnit.Analyzers.ClassicModelAssertUsage
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    [Shared]
    public sealed class IsInstanceOfClassicModelAssertUsageCodeFix
        : ClassicModelAssertUsageCodeFix
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(AnalyzerIdentifiers.IsInstanceOfUsage);

        protected override (ArgumentSyntax ActualArgument, ArgumentSyntax ConstraintArgument) ConstructActualAndConstraintArguments(
            Diagnostic diagnostic,
            IReadOnlyDictionary<string, ArgumentSyntax> argumentNamesToArguments,
            TypeArgumentListSyntax typeArguments)
        {
            var constraintArgument = SyntaxFactory.Argument(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(NUnitFrameworkConstants.NameOfIs),
                        SyntaxFactory.GenericName(NUnitFrameworkConstants.NameOfIsInstanceOf)
                            .WithTypeArgumentList(typeArguments))));
            var actualArgument = argumentNamesToArguments[NUnitFrameworkConstants.NameOfActualParameter].WithNameColon(null);
            return (actualArgument, constraintArgument.WithTriviaFrom(actualArgument));
        }

        protected override (ArgumentSyntax ActualArgument, ArgumentSyntax? ConstraintArgument) ConstructActualAndConstraintArguments(
            Diagnostic diagnostic,
            IReadOnlyDictionary<string, ArgumentSyntax> argumentNamesToArguments)
        {
            var expectedArgument = argumentNamesToArguments[NUnitFrameworkConstants.NameOfExpectedParameter].WithNameColon(null);
            var constraintArgument = SyntaxFactory.Argument(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(NUnitFrameworkConstants.NameOfIs),
                        SyntaxFactory.IdentifierName(NUnitFrameworkConstants.NameOfIsInstanceOf)))
                .WithArgumentList(SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(expectedArgument))));

            var actualArgument = argumentNamesToArguments[NUnitFrameworkConstants.NameOfActualParameter].WithNameColon(null);
            return (actualArgument, constraintArgument.WithTriviaFrom(actualArgument));
        }
    }
}
