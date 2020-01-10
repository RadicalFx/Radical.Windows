using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.CommandBuilders;
using System.Windows;

namespace Radical.Windows.Tests
{
    [TestClass]
    public class DelegateCommandBuilderTests
    {
        class TestDataContext
        {
            public TestDataContext()
            {
                MyProperty = new NestedPropertyClass();
            }

            public NestedPropertyClass MyProperty { get; private set; }

            public void DoSomething() { }

            public void DoSomethingCommand() { }

            public void EndsWithCommand() { }

            public void EndsWithcommand() { }

            public void WithoutCommandSuffix() { }
        }

        class NestedPropertyClass
        {
            public void DoSomethingElse() { }

            public void DoSomethingElseCommand() { }

            public void OtherThatEndsWithCommand() { }

            public void OtherThatEndsWithcommand() { }
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_simple_method_should_generate_CommandData()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("DoSomething"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_simple_method_with_Command_suffix_should_generate_CommandData()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("DoSomethingCommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_simple_method_with_no_Command_suffix_should_generate_CommandData_with_exact_match()
        {
            var sut = new DelegateCommandBuilder();
            var dc = new TestDataContext();
            var methodToInvoke = "DoSomething";

            var succeeded = sut.TryGenerateCommandData(new PropertyPath(methodToInvoke), dc, out CommandData cd);

            cd.FastDelegate(dc, null);

            Assert.AreEqual(methodToInvoke, cd.MethodName);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_simple_method_with_Command_suffix_should_generate_CommandData_with_exact_match()
        {
            var sut = new DelegateCommandBuilder();
            var dc = new TestDataContext();
            var methodToInvoke = "DoSomethingCommand";

            var succeeded = sut.TryGenerateCommandData(new PropertyPath(methodToInvoke), dc, out CommandData cd);

            cd.FastDelegate(dc, null);

            Assert.AreEqual(methodToInvoke, cd.MethodName);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_simple_method_on_nested_property_should_generate_CommandData()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("MyProperty.DoSomethingElse"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_simple_method_on_nested_property_Command_suffix_should_generate_CommandData()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("MyProperty.DoSomethingElseCommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_invalid_path_should_not_fail()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("ThisIsInvalid"), new TestDataContext(), out CommandData cd);

            Assert.IsFalse(succeeded);
            Assert.IsNull(cd);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_invalid_nested_path_should_not_fail()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("MyProperty.ThisIsInvalid"), new TestDataContext(), out CommandData cd);

            Assert.IsFalse(succeeded);
            Assert.IsNull(cd);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_invalid_full_nested_path_should_not_fail()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("Invalid.ThisIsInvalid"), new TestDataContext(), out CommandData cd);

            Assert.IsFalse(succeeded);
            Assert.IsNull(cd);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_PropertyPath_that_ends_with_Command_should_succeed()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("EndsWithCommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_nested_PropertyPath_that_ends_with_Command_should_succeed()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("MyProperty.OtherThatEndsWithCommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_PropertyPath_that_ends_with_command_lowercase_should_succeed()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("EndsWithcommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_nested_PropertyPath_that_ends_with_command_lowercase_should_succeed()
        {
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("MyProperty.OtherThatEndsWithcommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }

        [TestMethod]
        [TestCategory("DelegateCommandBuilder")]
        public void DelegateCommandBuilder_using_PropertyPath_that_ends_with_Command_but_method_doesnt_should_succeed()
        {
            //this is for backward compatibility
            var sut = new DelegateCommandBuilder();

            var succeeded = sut.TryGenerateCommandData(new PropertyPath("WithoutCommandSuffixCommand"), new TestDataContext(), out CommandData cd);

            Assert.IsTrue(succeeded);
            Assert.IsTrue(cd.FastDelegate != null);
        }
    }
}
