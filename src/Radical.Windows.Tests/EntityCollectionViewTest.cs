namespace Radical.Windows.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Radical.Model;
    using SharpTestsEx;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Threading;

    [TestClass()]
    public class EntityCollectionViewTest
    {
        [TestMethod]
        [Ignore]
        public void entityCollectionView_listChanged_event_in_multi_threaded_env_should_be_fired_in_the_creation_thread()
        {
            AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext();

            var expected = Thread.CurrentThread.ManagedThreadId;
            var actual = 0;
            var wh = new EventWaitHandle(false, EventResetMode.ManualReset);

            var list = new MementoEntityCollection<GenericParameterHelper>();
            var view = list.DefaultView;
            view.ListChanged += (s, e) =>
            {
                actual = Thread.CurrentThread.ManagedThreadId;
                //wh.Set();
            };

            new Thread(new ThreadStart(() =>
          {
              list.Add(new GenericParameterHelper());
          })).Start();

            actual.Should().Be.EqualTo(expected);
        }
    }
}
