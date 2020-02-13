using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radical.Windows.ComponentModel;
using Radical.Windows.Presentation;
using Radical.Windows.Regions;
using Radical.Windows.Tests;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Test.Radical.Windows.Presentation.Regions.Specialized
{
    [TestClass]
    public class TacControlRegionTests
    {
        class TestTabControlRegion : TabControlRegion
        {
            public TestTabControlRegion()
            {
                TestHostingView = new Window();
            }

            public DependencyObject TestHostingView { get; set; }

            protected override DependencyObject FindHostingViewOf( FrameworkElement fe )
            {
                return TestHostingView;
            }

            protected override object TryGetViewModel( DependencyObject view )
            {
                return ( ( FrameworkElement )view ).DataContext;
            }
        }

        class TestViewModel : AbstractViewModel, IExpectViewActivatedCallback
        {

            public void OnViewActivated()
            {
                Invoked = true;
            }

            public bool Invoked { get; private set; }
        }

        class HardCodedServiceProvider : IServiceProvider
        {
            readonly IProvideValueTarget ipvt = new IPVT()
            {
                TargetObject = new TabControl()
            };

            public HardCodedServiceProvider()
            {

            }

            public object GetService( Type serviceType )
            {
                return ipvt;
            }
        }

        class IPVT : IProvideValueTarget
        {
            public object TargetObject
            {
                get;
                set;
            }

            public object TargetProperty
            {
                get;
                set;
            }
        }

        [TestMethod]
        [TestCategory( "TabControlRegion" ), TestCategory( "Regions" ), TestCategory( "UI Composition" )]
        public void TabControlRegion_ActiveContentChanged_should_notify_VM_if_IExpectViewActivatedCallback()
        {
            var sut = new TestTabControlRegion();
            sut.ProvideValue(new HardCodedServiceProvider());

            var vm1 = new TestViewModel();
            var item1 = new UserControl()
            {
                DataContext = vm1
            };
            sut.Add(item1);

            var vm2 = new TestViewModel();
            var item2 = new UserControl()
            {
                DataContext = vm2
            };
            sut.Add(item2);

            sut.Activate(item2);
            sut.Activate(item1);

            Assert.IsTrue(vm1.Invoked);
            Assert.IsTrue(vm2.Invoked);
        }
    }
}
