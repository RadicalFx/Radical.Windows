using System.Windows.Data;

namespace Radical.Windows.Markup
{
    /// <summary>
    /// A binding for editors that validates on data errors and exceptions, and updates the source when the target property changes.
    /// </summary>
    public class EditorBinding : BindingDecoratorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBinding"/> class.
        /// </summary>
        public EditorBinding()
            : base()
        {
            InitDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorBinding"/> class.
        /// </summary>
        /// <param name="path">The binding path.</param>
        public EditorBinding(string path)
            : base(path)
        {
            InitDefaults();
        }

        void InitDefaults()
        {
            NotifyOnValidationError = true;
            ValidatesOnDataErrors = true;
            ValidatesOnExceptions = true;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}