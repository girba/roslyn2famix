namespace Fame.Fm3
{
    using Common;

    /// <summary>
    /// Abstract superclass of MSE metamodel.
    /// 
    /// This is an abstract class with attributes
    /// <ul>
    /// <li>Element <code>owner</code> (derived)</li>
    /// <li>String <code>fullname</code> (derived)</li>
    /// <li>String <code>name</code></li>
    /// </ul>
    /// <p>
    /// with these constraints
    /// </p>
    /// <ul>
    /// <li> <code>name</code> must be alphanumeric</li>
    /// <li> <code>fullname</code> is derived recursively, concatenating
    /// <code>owner.fullname</code> and <code>name</code></li>
    /// <li> <code>fullname</code> is separated by dots, eg
    /// <code>MSE.Class.attributes</code></li>
    /// </ul>
    /// 
    /// @author Adrian Kuhn
    /// </summary>
    [FamePackage("FM3")]
    [FameDescription("Element")]
    public abstract class Element : INamed, INested
    {
        public Element()
        {
        }

        public Element(string name)
        {
            Name = name;
        }

        [FamePropertyWithDerived]
        public string Fullname
        {
            get
            {
                var parent = GetOwner();
                return parent == null ? Name : parent.Fullname + "." + Name;
            }
        }

        [FameProperty]
        public string Name { get; set; } = string.Empty;

        [FamePropertyWithDerived]
        public Element GetOwner()
        {
            return null;
        }

        public override string ToString()
        {
            return Fullname;
        }

        public abstract void CheckContraints(Warnings warnings);
    }
}
