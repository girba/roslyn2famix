namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Solution : IFamixLanguageNode
    {
        public Solution(string name)
        {
            this.Name = name;

            this.Projects = new List<Project>();
        }

        public string Name { get; }

        public IList<Project> Projects { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Solution {this.Name})");

            foreach (var project in this.Projects)
            {
                famixBuilder.AppendLine(project.ToString());
            }

            famixBuilder.AppendLine($"(Solution {this.Name} end)");

            return famixBuilder.ToString();
        }
    }
}
