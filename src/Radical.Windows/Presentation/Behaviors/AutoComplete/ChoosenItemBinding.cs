using System;

namespace Radical.Windows.Markup
{
    public class ChoosenItemBinding : BindingDecoratorBase
    {
        public ChoosenItemBinding()
            : base()
        {
            this.InitDefaults();
        }

        public ChoosenItemBinding(String path)
            : base(path)
        {
            this.InitDefaults();
        }

        void InitDefaults()
        {
            this.Mode = System.Windows.Data.BindingMode.OneWayToSource;
            this.UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }
    }
}
