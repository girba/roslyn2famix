namespace Famix.Language
{
    public class Method
    {
        private readonly string name;

        public Method(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return $"(Method {this.name})";
        }
    }
}
