namespace TypeVisualiserUnitTests
{
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser;

    [TestClass]
    public class TrivialFilterTest
    {
        [TestMethod]
        public void SaveXmlListTest()
        {
            var runtimeSettings = new TrivialFilter();
            var accessor = TrivialFilter_Accessor.AttachShadow(runtimeSettings);
            accessor.trivialTypes.Add(new ExactMatch() { FullTypeName = "System.Object" });
            accessor.trivialTypes.Add(new ExactMatch() { FullTypeName = "System.String" });
            accessor.SaveTrivialExcludeXml();
            Debug.WriteLine(accessor.FullTrivialListXmlFileName);
        }
    }
}
