namespace Radical.Windows.Tests
{
    using FakeItEasy;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Radical;
    using Radical.ComponentModel.ChangeTracking;
    using Radical.Observers;
    using Radical.Windows.Input;
    using SharpTestsEx;
    using System.ComponentModel;

    [TestClass]
    public class DelegateCommandTests
    {
        class TestStub : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }

            private string _value = null;
            public string Value
            {
                get { return _value; }
                set
                {
                    if (value != Value)
                    {
                        _value = value;
                        OnPropertyChanged("Value");
                    }
                }
            }

            private string _anotherValue = null;
            public string AnotherValue
            {
                get { return _anotherValue; }
                set
                {
                    if (value != AnotherValue)
                    {
                        _anotherValue = value;
                        OnPropertyChanged("AnotherValue");
                    }
                }
            }

            private readonly Observable<string> _text = new Observable<string>();
            public Observable<string> Text
            {
                get { return _text; }
            }
        }

        [TestMethod]
        public void delegateCommand_trigger_using_mementoMonitor_and_manually_calling_notifyChanged_should_raise_CanExecuteChanged()
        {
            var expected = 1;
            var actual = 0;

            var svc = A.Fake<IChangeTrackingService>();
            var monitor = new MementoMonitor(svc);

            var target = DelegateCommand.Create().AddMonitor(monitor);
            target.CanExecuteChanged += (s, e) => actual++;
            monitor.NotifyChanged();

            actual.Should().Be.EqualTo(expected);
        }

        [TestMethod]
        public void delegateCommand_trigger_using_mementoMonitor_and_triggering_changes_on_the_memento_should_raise_canExecuteChanged()
        {
            var expected = 1;
            var actual = 0;

            var svc = A.Fake<IChangeTrackingService>();
            var monitor = new MementoMonitor(svc);

            var target = DelegateCommand.Create().AddMonitor(monitor);
            target.CanExecuteChanged += (s, e) => actual++;

            svc.TrackingServiceStateChanged += Raise.WithEmpty();

            actual.Should().Be.EqualTo(expected);
        }

        [TestMethod]
        public void delegateCommand_trigger_using_PropertyObserver_ForAllproperties_should_trigger_canExecuteChanged()
        {
            var expected = 2;
            var actual = 0;

            var stub = new TestStub();

            var target = DelegateCommand.Create().Observe(stub);
            target.CanExecuteChanged += (s, e) => actual++;

            stub.Value = "this raises PropertyChanged";
            stub.AnotherValue = "this raises PropertyChanged";

            actual.Should().Be.EqualTo(expected);
        }

        [TestMethod]
        public void delegateCommand_trigger_using_PropertyObserver_For_a_property_should_trigger_canExecuteChanged()
        {
            var expected = 1;
            var actual = 0;

            var stub = new TestStub();

            var target = DelegateCommand.Create().Observe(stub, s => s.Value);
            target.CanExecuteChanged += (s, e) => actual++;

            stub.Value = "this raises PropertyChanged";
            stub.AnotherValue = "this raises PropertyChanged";

            actual.Should().Be.EqualTo(expected);
        }
    }
}
