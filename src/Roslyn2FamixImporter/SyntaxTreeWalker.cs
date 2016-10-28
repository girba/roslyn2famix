namespace Roslyn2FamixImporter
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Famix;

    public class SyntaxTreeWalker : CSharpSyntaxWalker
    {
        private readonly FamixTreeBuilder builder;

        public SyntaxTreeWalker(FamixTreeBuilder builder)
        {
            this.builder = builder;
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var className = node.Identifier.ToString();
            this.builder.BeginClass(className);

            base.VisitClassDeclaration(node);

            this.builder.EndClass(className);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodName = node.Identifier.ToString();
            this.builder.BeginMethod(methodName);

            base.VisitMethodDeclaration(node);

            this.builder.EndMethod(methodName);
        }
    }
}
