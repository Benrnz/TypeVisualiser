namespace TypeVisualiserUnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using GalaSoft.MvvmLight.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;
    using TypeVisualiser;
    using TypeVisualiser.DemoTypes;
    using TypeVisualiser.Messaging;
    using TypeVisualiser.UI;
    using TypeVisualiser.UI.WpfUtilities;

    [TestClass]
    public class ChooseTypeControllerTest
    {
        internal enum TestEnum
        {
            NotSet,
            Something,
            SomethingElse,
        }

        internal interface TestInternalInterface
        {
        }

        internal class TestInternalClass
        {
        }

        private Type chosenType;
        private IMessenger mockMessenger;
        private MockRepository mockery;

        private ChooseTypeController CreateChooseTypeController()
        {
            Assembly assemblyToLoadFrom = GetType().Assembly;
            Action<Type> onTypeChosen = type => { this.chosenType = type; };
            var target = new ChooseTypeController(assemblyToLoadFrom, onTypeChosen);
            if (this.mockMessenger != null)
            {
                var accessor = TypeVisualiserControllerBase_Accessor.AttachShadow(target);
                accessor.doNotUseMessenger = this.mockMessenger;
            }

            return target;
        }

        [TestInitialize]
        public void TestSetup()
        {
            this.mockery = new MockRepository();
        }

        [TestMethod]
        public void ConstructorShouldInitialiseCollection()
        {
            var target = CreateChooseTypeController();
            Assert.IsNotNull(target.Types);
        }

        [TestMethod]
        public void ConstructorShouldPopulateTypesCollection()
        {
            var target = CreateChooseTypeController();
            var accessor = ChooseTypeController_Accessor.AttachShadow(target);
            accessor.loadTypesTask.Wait();

            DispatcherHelper.DoEvents();
            Assert.IsTrue(target.Types.Any());
        }

        [TestMethod]
        public void GetImagePathShouldReturnUniqueValuesFor6Scenarios()
        {
            var testTypes = new[] { typeof(TestInternalClass), typeof(TestEnum), typeof(TestInternalInterface), typeof(string), typeof(Visibility), typeof(ICloneable) };
            var results = testTypes.Select(ChooseTypeController_Accessor.GetImagePath).ToList();

            Assert.IsTrue(results.Any());
            Assert.AreEqual(6, results.Distinct().Count());
        }

        [TestMethod]
        public void ChangeAssemblyExecutedShouldSendChooseAssemblyMessage()
        {
            using (this.mockery.Record())
            {
                this.mockMessenger = this.mockery.StrictMock<IMessenger>();
                this.mockMessenger.Expect(m => m.Send<ChooseAssemblyMessage>(null)).IgnoreArguments();
            }

            var target = CreateChooseTypeController();
            var accessor = ChooseTypeController_Accessor.AttachShadow(target);
            accessor.ChangeAssemblyExecuted();

            using (this.mockery.Playback())
            {
            }
        }

        [TestMethod]
        public void ChangeAssemblyExecutedShouldRequestClose()
        {
            var target = CreateChooseTypeController();
            bool eventRaised = false;
            target.CloseRequested += (s, e) => { eventRaised = true; };
            var accessor = ChooseTypeController_Accessor.AttachShadow(target);
            accessor.ChangeAssemblyExecuted();

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void TypeChosenShouldRequestClose()
        {
            var target = CreateChooseTypeController();
            bool eventRaised = false;
            target.CloseRequested += (s, e) => { eventRaised = true; };
            var accessor = ChooseTypeController_Accessor.AttachShadow(target);
            accessor.TypeChosenExecuted("System.String");

            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void TypeChosenShouldExecuteGivenAction()
        {
            var target = CreateChooseTypeController();
            var accessor = ChooseTypeController_Accessor.AttachShadow(target);
            accessor.TypeChosenExecuted("TypeVisualiserUnitTests.ChooseTypeControllerTest");

            Assert.AreEqual(GetType(), this.chosenType);
        }
    }
}