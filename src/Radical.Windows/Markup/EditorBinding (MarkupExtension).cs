using System;
using System.Windows.Data;

namespace Radical.Windows.Markup
{
    public class EditorBinding : BindingDecoratorBase
    {
        public EditorBinding()
            : base()
        {
            InitDefaults();
        }

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