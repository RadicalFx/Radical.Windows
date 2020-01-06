using Radical.Validation;
using System;
using System.Windows;

namespace Radical.Windows.Behaviors
{
    /// <summary>
    /// The base abstract class used by Drag 'n' Drop event arguments.
    /// </summary>
    public abstract class DragDropOperationArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropOperationArgs"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyStates">The key states.</param>
        /// <param name="dropTarget">The drop target.</param>
        protected DragDropOperationArgs(IDataObject data, DragDropKeyStates keyStates, object dropTarget)
        {
            Ensure.That(data).Named("data").IsNotNull();
            Ensure.That(keyStates).Named("keyStates").IsTrue(ks => ks.IsDefined());

            Data = data;
            KeyStates = keyStates;
            DropTarget = dropTarget;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public IDataObject Data { get; private set; }

        /// <summary>
        /// Gets the key states.
        /// </summary>
        /// <value>The key states.</value>
        public DragDropKeyStates KeyStates { get; private set; }

        /// <summary>
        /// Gets or sets the drop target.
        /// </summary>
        /// <value>The drop target.</value>
        public object DropTarget { get; private set; }
    }
}
