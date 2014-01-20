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
        private IFileManager mockFileManager;
        private Diagram fakeDiagram;
        private IDiagramController mockController;
        private ShellController target;
        private static IVisualisableTypeWithAssociations demoType;

        private static IApplicationResources resources;

        [TestMethod]
        [Description("This test is to prevent a bug from reoccuring. Refreshing a loaded visualisable type crashed.")]
        public void RefreshExecuteShouldReloadTypeWithNoParent()
        {
            this.mockController.Expect(m => m.Refresh(this.mockFileManager));
            PrivateAccessor.SetProperty(this.fakeDiagram, "IsLoaded", true);

            this.target.RefreshCommand.Execute(null);

            this.mockController.VerifyAllExpectations();
        }

        [ClassInitialize]
        public static void ClassInitialise(TestContext context)
        {
            demoType = VisualisableTypeTestData.FullModel<Car>(new Container());
        }

        [TestInitialize]
        public void TestInitialise()
        {
            this.factory = new Container();
            this.mockFileManager = MockRepository.GenerateMock<IFileManager>();
            
            this.mockController = MockRepository.GenerateMock<IDiagramController>();
            this.fakeDiagram = new Diagram(this.mockController);

            factory.Configure(config => config.For<IFileManager>().Use(this.mockFileManager));

            this.mockFileManager.Expect(m => m.LoadDemoType()).IgnoreArguments().Return(demoType);

            var harness = new ShellControllerTestHarness(factory)
                              {
                                  CreateController = () => this.mockController,
                                  CurrentView = this.fakeDiagram,
                              };
            this.target = harness;
        }
    }
}