using Radical.ComponentModel;
using Radical.Validation;
using System;
using System.Windows;

namespace Radical.Windows
{
    public class Fact : IMonitor, IWeakEventListener
    {
        public static Fact Create(Func<object, bool> evaluator)
        {
            return new Fact(evaluator);
        }

        readonly Func<object, bool> evaluator;

        public Fact(Func<object, bool> evaluator)
        {
            this.evaluator = evaluator;
        }

        public bool Eval(object parameter)
        {
            return evaluator(parameter);
        }

        public event EventHandler Changed;

        public void NotifyChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        public static implicit operator bool(Fact fact)
        {
            return fact.Eval(null);
        }

        public Fact AddMonitor(IMonitor source)
        {
            Ensure.That(source).Named("source").IsNotNull();

            MonitorChangedWeakEventManager.AddListener(source, this);

            return this;
        }

        public Fact RemoveMonitor(IMonitor source)
        {
            Ensure.That(source).Named("source").IsNotNull();
            MonitorChangedWeakEventManager.RemoveListener(source, this);

            return this;
        }

        void OnMonitorChanged(IMonitor source)
        {
            NotifyChanged();
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(MonitorChangedWeakEventManager))
            {
                OnMonitorChanged((IMonitor)sender);
            }
            else
            {
                // unrecognized event
                return false;
            }

            return true;
        }

        void IMonitor.StopMonitoring()
        {
            //NOP
        }
    }
}
