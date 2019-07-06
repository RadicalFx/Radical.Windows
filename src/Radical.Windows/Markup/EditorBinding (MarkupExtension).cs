using System;
using System.Windows.Data;

namespace Radical.Windows.Markup
{
    public class EditorBinding : BindingDecoratorBase
    {
        public EditorBinding()
            : base()
        {
            this.InitDefaults();
        }

        public EditorBinding(String path)
            : base(path)
        {
            this.InitDefaults();
        }

        void InitDefaults()
        {
            this.NotifyOnValidationError = true;
            this.ValidatesOnDataErrors = true;
            this.ValidatesOnExceptions = true;
            this.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
}