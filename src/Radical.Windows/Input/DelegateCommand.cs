﻿using Radical.Analytics;
using Radical.ComponentModel;
using Radical.Validation;
using Radical.Windows.Analytics;
using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Radical.Windows.Input
{
    /// <summary>
    /// The default implementation of the IDelegateCommand interface.
    /// </summary>
    public class DelegateCommand : IDelegateCommand, IWeakEventListener
    {
        object _data;

        /// <summary>
        /// Used by the AutoCommandBinding...shuold be rewritten better :-)
        /// </summary>
        /// <param name="data">The data.</param>
        internal void SetData(object data)
        {
            _data = data;
        }

        /// <summary>
        /// Used by the AutoCommandBinding...shuold be rewritten better :-)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T GetData<T>()
        {
            return (T)_data;
        }

        /// <summary>
        /// An emtpy command, usefull as a placeholder.
        /// </summary>
        public static readonly IDelegateCommand Empty =
            Create()
            .OnCanExecute(o => false);

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <returns>The new command.</returns>
        public static IDelegateCommand Create()
        {
            return new DelegateCommand(null, null, string.Empty);
        }

        /// <summary>
        /// Creates a new command with the specified display text.
        /// </summary>
        /// <param name="displayText">The display text.</param>
        /// <returns>The new command.</returns>
        public static IDelegateCommand Create(string displayText)
        {
            return new DelegateCommand(null, null, displayText);
        }

        private Action<object> executeMethod = null;
        private Func<object, bool> canExecuteMethod = null;

        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises <seealso cref="CanExecuteChanged"/> on the UI thread.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        private InputBindingCollection inputBindings;

        /// <summary>
        /// Constructor. Initializes delegate command with Execute delegate and CanExecute delegate
        /// </summary>
        /// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        /// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
        /// <param name="displayText">Text displayed by elements this command is bound to</param>
        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod, string displayText)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
            DisplayText = displayText;
        }

        /// <summary>
        /// Sets the given Action as the delegate that must handle
        /// the commands execution logic.
        /// </summary>
        /// <param name="executeMethod">The delegate to execute at execution time.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public virtual IDelegateCommand OnExecute(Action<object> executeMethod)
        {
            this.executeMethod = executeMethod;
            return this;
        }

        /// <summary>
        /// Sets the given Action as the delegate that must handle
        /// the logic that determines whether the command can be executed or not.
        /// </summary>
        /// <param name="canExecuteMethod">The delegate to invoke.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public virtual IDelegateCommand OnCanExecute(Func<object, bool> canExecuteMethod)
        {
            this.canExecuteMethod = canExecuteMethod;
            return this;
        }

        /// <summary>
        /// Adds a new gesture to associate input bindings
        /// </summary>
        public IDelegateCommand AddGesture(InputGesture gesture)
        {
            if (inputBindings == null)
            {
                inputBindings = new InputBindingCollection();
            }

            inputBindings.Add(new InputBinding(this, gesture));

            return this;
        }

        public IDelegateCommand AddKeyGesture(System.Windows.Input.Key key)
        {
            return AddKeyGesture(key, ModifierKeys.None);
        }

        public IDelegateCommand AddKeyGesture(System.Windows.Input.Key key, ModifierKeys mk)
        {
            if (inputBindings == null)
            {
                inputBindings = new InputBindingCollection();
            }

            inputBindings.Add(new InputBinding(this, new KeyGesture(key, mk)));

            return this;
        }

        /// <summary>
        /// Gets command display text
        /// </summary>
        public string DisplayText
        {
            get;
            private set;
        }

        /// <summary>
        /// Command's associated input bindings
        /// </summary>
        public InputBindingCollection InputBindings
        {
            get { return inputBindings; }
        }

        ///<summary>
        ///Defines the method that determines whether the command can execute in its current state.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        ///<returns>
        ///true if this command can be executed; otherwise, false.
        ///</returns>
        public bool CanExecute(object parameter)
        {
            if (canExecuteMethod == null)
            {
                return true;
            }

            return canExecuteMethod(parameter);
        }

        ///<summary>
        ///Defines the method to be called when the command is invoked.
        ///</summary>
        ///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (executeMethod == null)
            {
                return;
            }

            executeMethod(parameter);

            if (onBeforeTracking != null)
            {
                onBeforeTracking(new CommandEvent()
                {
                    Name = DisplayText,
                    Data = new Dictionary<string, object>()
                    {
                        {"parameter", parameter}
                    }
                });
            }
        }

        Action<AnalyticsEvent> onBeforeTracking;

        /// <summary>
        /// Intercepts the analytics tracking information before sending to the analytics service.
        /// </summary>
        /// <param name="onBeforeTracking">The before tracking handler.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand WithTrackingPreview(Action<AnalyticsEvent> onBeforeTracking)
        {
            this.onBeforeTracking = onBeforeTracking;

            return this;
        }

        /// <summary>
        /// Raises <see cref="CanExecuteChanged"/> so every command invoker can re-query to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        public virtual void EvaluateCanExecute()
        {
            OnCanExecuteChanged();
        }

        /// <summary>
        /// Adds all the given triggers to the list of triggers.
        /// </summary>
        /// <param name="triggers">The triggers.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand AddMonitor(params IMonitor[] triggers)
        {
            if (triggers != null)
            {
                foreach (var monitor in triggers)
                {
                    AddMonitor(monitor);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds a trigger monitor to the list of triggers.
        /// </summary>
        /// <param name="source">The source monitor.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand AddMonitor(IMonitor source)
        {
            Ensure.That(source).Named("source").IsNotNull();
            MonitorChangedWeakEventManager.AddListener(source, this);

            return this;
        }

        /// <summary>
        /// Removes the given monitor from the list of triggers.
        /// </summary>
        /// <param name="source">The monitor to remove.</param>
        /// <returns>
        /// An instance of the current command.
        /// </returns>
        public IDelegateCommand RemoveMonitor(IMonitor source)
        {
            Ensure.That(source).Named("source").IsNotNull();
            MonitorChangedWeakEventManager.RemoveListener(source, this);

            return this;
        }

        /// <summary>
        /// Called one of the trigger registered with this command
        /// raises the changed event.
        /// </summary>
        /// <param name="source">The source trigger.</param>
        protected virtual void OnTriggerChanged(IMonitor source)
        {
            EvaluateCanExecute();
        }

        /// <summary>
        /// Receives events from the centralized event manager.
        /// </summary>
        /// <param name="managerType">The type of the <see cref="T:System.Windows.WeakEventManager"/> calling this method.</param>
        /// <param name="sender">Object that originated the event.</param>
        /// <param name="e">Event data.</param>
        /// <returns>
        /// true if the listener handled the event. It is considered an error by the <see cref="T:System.Windows.WeakEventManager"/> handling in WPF to register a listener for an event that the listener does not handle. Regardless, the method should return false if it receives an event that it does not recognize or handle.
        /// </returns>
        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
        {
            if (managerType == typeof(MonitorChangedWeakEventManager))
            {
                OnTriggerChanged((IMonitor)sender);
            }
            else
            {
                // unrecognized event
                return false;
            }

            return true;
        }
    }
}
