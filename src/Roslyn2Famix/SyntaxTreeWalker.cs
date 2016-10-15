namespace Roslyn2Famix
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
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var methodName = node.Identifier.ToString();
            this.builder.BeginMethod(methodName);

            base.VisitMethodDeclaration(node);
        }
    }
}
