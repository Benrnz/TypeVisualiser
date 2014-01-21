namespace TypeVisualiserUnitTests.Controllers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using StructureMap;
    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Model;
    using TypeVisualiser.UI;

    [TestClass]
    public class ShellControllerTest
    {
        private IContainer factory;
        private IDiagramDimensions mockDimensions;
        private IFileManager mockFileManager;
        private Diagram fakeDiagram;
        private IDiagramController mockController;
        private ShellController target;
        private VisualisableTypeWithAssociations demoType = new VisualisableTypeWithAssociations(typeof(Car), 0);
        private ITrivialFilter mockTrivialFilter;

        [TestMethod]
        public void RefreshExecuteShouldReloadTypeWithNoParent()
        {
            this.mockController.Expect(m => m.Refresh(this.mockFileManager));
            PrivateAccessor.SetProperty(this.fakeDiagram, "IsLoaded", true);

            this.target.RefreshCommand.Execute(null);

            this.mockController.VerifyAllExpectations();
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.mockFileManager = MockRepository.GenerateMock<IFileManager>();
            this.mockDimensions = MockRepository.GenerateStub<IDiagramDimensions>();
            this.mockController = MockRepository.GenerateMock<IDiagramController>();
            this.mockTrivialFilter = MockRepository.GenerateStub<ITrivialFilter>();
            this.fakeDiagram = new Diagram(this.mockController);

            this.factory = new Container(
                config =>
                {
                    config.For<IDiagramDimensions>().Use(this.mockDimensions);
                    config.For<IVisualisableTypeWithAssociations>().Use<VisualisableTypeWithAssociations>();
                    config.For<IFileManager>().Use(this.mockFileManager);
                });

            this.mockFileManager.Expect(m => m.LoadDemoType()).IgnoreArguments().Return(demoType);

            var harness = new ShellControllerTestHarness(this.factory)
                              {
                                  CreateController = () => this.mockController, 
                                  CurrentView = this.fakeDiagram,
                              };
            this.target = harness;

            PrivateAccessor.SetStaticField(typeof(TrivialFilter), "current", this.mockTrivialFilter);
        }
    }
}