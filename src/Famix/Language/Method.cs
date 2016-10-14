namespace Famix.Language
{
    public class Method
    {
        public Method(string name)
        {
            this.Name = name;
        }
        
        public string Name { get; }
        
        public override string ToString()
        {
            return $"(Method {this.Name})";
        }
    }
}
