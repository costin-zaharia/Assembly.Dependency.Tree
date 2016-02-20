namespace DependencyTree.Controls
{
    public class AssemblyInfoViewModel
    {
        public string Description { get; set; }

        public string DependenciesDescription { get; set; }

        public AssemblyInfoViewModel(string description, string dependenciesDescription)
        {
            Description = description;
            DependenciesDescription = dependenciesDescription;
        }
    }
}
