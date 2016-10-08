namespace Roslyn2Famix
{
    using Famix;
    using Microsoft.CodeAnalysis;

    public class SymbolWalker : SymbolVisitor
    {
        private readonly FamixTreeBuilder builder;

        public SymbolWalker(FamixTreeBuilder builder)
        {
            this.builder = builder;
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var childSymbol in symbol.GetMembers())
            {
                childSymbol.Accept(this);
            }

            base.VisitNamespace(symbol);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            var className = symbol.Name;
            this.builder.CreateClass(className);

            foreach (var childSymbol in symbol.GetMembers())
            {
                childSymbol.Accept(this);
            }

            base.VisitNamedType(symbol);
        }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            if (!symbol.IsImplicitlyDeclared)
            {
                var methodName = symbol.Name;
                this.builder.CreateMethod(methodName);
            }

            base.VisitMethod(symbol);
        }
    }
}
