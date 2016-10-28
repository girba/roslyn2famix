namespace Roslyn2FamixImporter
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

        public override void VisitAssembly(IAssemblySymbol assemblySymbol)
        {
            builder.BeginAssembly(assemblySymbol.Name);

            foreach (var childSymbol in assemblySymbol.Modules)
            {
                childSymbol.Accept(this);
            }

            builder.EndAssembly(assemblySymbol.Name);

            base.VisitAssembly(assemblySymbol);
        }

        public override void VisitModule(IModuleSymbol moduleSymbol)
        {
            var globalNamespace = moduleSymbol.GlobalNamespace;
            globalNamespace.Accept(this);

            base.VisitModule(moduleSymbol);
        }

        public override void VisitNamespace(INamespaceSymbol namespaceSymbol)
        {
            builder.BeginNamespace(namespaceSymbol.Name);

            foreach (var childSymbol in namespaceSymbol.GetMembers())
            {
                childSymbol.Accept(this);
            }

            builder.EndNamespace(namespaceSymbol.Name);

            base.VisitNamespace(namespaceSymbol);
        }

        public override void VisitNamedType(INamedTypeSymbol namedTypeSymbol)
        {
            this.builder.BeginClass(namedTypeSymbol.Name);

            foreach (var childSymbol in namedTypeSymbol.GetMembers())
            {
                childSymbol.Accept(this);
            }

            this.builder.EndClass(namedTypeSymbol.Name);

            base.VisitNamedType(namedTypeSymbol);
        }

        public override void VisitMethod(IMethodSymbol methodSymbol)
        {
            if (!methodSymbol.IsImplicitlyDeclared)
            {
                this.builder.BeginMethod(methodSymbol.Name);
                this.builder.EndMethod(methodSymbol.Name);
            }

            base.VisitMethod(methodSymbol);
        }
    }
}
