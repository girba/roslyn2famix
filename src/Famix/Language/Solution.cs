namespace Famix.Language
{
    using Famix.Language.Contracts;
    using System.Collections.Generic;
    using System.Text;

    public class Solution : IFamixNode, IFamixContainer<Project>
    {
        public Solution()
        {
            this.Name = string.Empty;

            this.Projects = new List<Project>();
        }

        public string Name { get; }

        public IList<Project> Projects { get; }

        public void Add(Project project)
        {
            this.Projects.Add(project);
        }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Solution)");

            foreach (var project in this.Projects)
            {
                famixBuilder.Append(project.ToString());
            }

            famixBuilder.AppendLine($"(Solution end)");

            return famixBuilder.ToString();
        }
    }
}
