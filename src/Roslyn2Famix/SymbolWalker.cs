namespace Roslyn2Famix
{
    using Famix.Language;
    using Microsoft.CodeAnalysis;

    public class SymbolWalker : SymbolVisitor
    {
        private Class rootClass = null;

        public string ToFamixString()
        {
            return rootClass?.ToString();
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
            this.rootClass = new Class(className);
            
            foreach(var childSymbol in symbol.GetMembers())
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
                var method = new Method(methodName);

                this.rootClass?.Methods.Add(method);
            }
            
            base.VisitMethod(symbol);
        }
    }
}
