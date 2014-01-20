namespace TypeVisualiserUnitTests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TypeVisualiser.Model;

    [TestClass]
    public class TypeDescriptorHelperTest
    {
        private Type GetInputTestType
        {
            get
            {
                return typeof(KeyValuePair<string, double>);
            }
        }

        private TypeDescriptorHelper GetTestSubject
        {
            get
            {
                return new TypeDescriptorHelper(GetInputTestType);
            }
        }

        [TestMethod]
        public void ShouldDetectGenericType()
        {
            Assert.IsTrue(GetTestSubject.IsGeneric);
        }

        [TestMethod]
        public void ShouldNotDetectGenericType()
        {
            Assert.IsFalse(new TypeDescriptorHelper(typeof(string)).IsGeneric);
        }

        [TestMethod]
        public void ShouldName1GenericTypes()
        {
            Assert.AreEqual("List<String>", new TypeDescriptorHelper(typeof(List<string>)).GenerateName());
        }

        [TestMethod]
        public void ShouldName2GenericTypes()
        {
            Assert.AreEqual("KeyValuePair<String,Double>", GetTestSubject.GenerateName());
        }

        [TestMethod]
        public void ShouldNotBeInLowerCase()
        {
            Assert.AreNotEqual("KeyValuePair<string,double>", GetTestSubject.GenerateName());
        }

        [TestMethod]
        public void ShouldName0GenericTypes()
        {
            Assert.AreEqual("Visibility", new TypeDescriptorHelper(typeof(Visibility)).GenerateName());
        }
    }
}
