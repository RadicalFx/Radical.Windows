using Radical.Validation;
using System.Windows;

namespace Radical.Windows.Behaviors
{
    public class DragOverArgs : DragDropOperationArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DragOverArgs" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="keyStates">The key states.</param>
        /// <param name="dropTarget">The drop target.</param>
        /// <param name="allowedEffects">The allowed effects.</param>
        /// <param name="position">The position.</param>
        public DragOverArgs(IDataObject data, DragDropKeyStates keyStates, object dropTarget, DragDropEffects allowedEffects, Point position)
            : base(data, keyStates, dropTarget)
        {
            Ensure.That(allowedEffects).Named("allowedEffects").IsTrue(v => v.IsDefined());

            AllowedEffects = allowedEffects;
            Position = position;
        }

        /// <summary>
        /// Gets the allowed effects.
        /// </summary>
        /// <value>The allowed effects.</value>
        public DragDropEffects AllowedEffects { get; private set; }

        /// <summary>
        /// Gets or sets the effects.
        /// </summary>
        /// <value>The effects.</value>
        public DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets the Position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position { get; private set; }
    }
}