namespace Roslyn2Famix
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Famix.Language;

    public class SyntaxTreeWalker : CSharpSyntaxWalker
    {
        private Class rootClass = null;

        public string ToFamixString()
        {
            return rootClass?.ToString();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var className = node.Identifier.ToString();
            this.rootClass = new Class(className);
            
            base.VisitClassDeclaration(node);
        }
    
        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {


            var methodName = node.Identifier.ToString();
            var method = new Method(methodName);

            this.rootClass?.Methods.Add(method);

            base.VisitMethodDeclaration(node);
        }
    }
}
