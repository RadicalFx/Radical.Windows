using System;

namespace Radical.Windows.Markup
{
    public class ChoosenItemBinding : BindingDecoratorBase
    {
        public ChoosenItemBinding()
            : base()
        {
            InitDefaults();
        }

        public ChoosenItemBinding(string path)
            : base(path)
        {
            InitDefaults();
        }

        void InitDefaults()
        {
            Mode = System.Windows.Data.BindingMode.OneWayToSource;
            UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }
    }
}
