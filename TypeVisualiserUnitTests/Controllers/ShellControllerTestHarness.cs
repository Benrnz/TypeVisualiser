using System;
using StructureMap;
using TypeVisualiser.Model;
using TypeVisualiser.UI;

namespace TypeVisualiserUnitTests.Controllers
{
    public class ShellControllerTestHarness : ShellController
    {
        public ShellControllerTestHarness(IContainer factory) : base(factory)
        {
        }

        public Func<IDiagramController> CreateController { get; set; }

        protected override Diagram CreateDiagram()
        {
            return new Diagram(CreateController());
        }
    }
}
