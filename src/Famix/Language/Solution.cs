﻿namespace Famix.Language
{
    using System.Collections.Generic;
    using System.Text;

    public class Solution : IFamixLanguageNode
    {
        private readonly string name;

        public Solution(string name)
        {
            this.name = name;

            this.Projects = new List<Project>();
        }

        public IList<Project> Projects { get; }

        public override string ToString()
        {
            var famixBuilder = new StringBuilder();

            famixBuilder.AppendLine($"(Solution {this.name})");

            foreach (var project in this.Projects)
            {
                famixBuilder.AppendLine(project.ToString());
            }

            famixBuilder.AppendLine($"(Solution {this.name} end)");

            return famixBuilder.ToString();
        }
    }
}
