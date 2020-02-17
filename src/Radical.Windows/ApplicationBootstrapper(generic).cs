using System;

namespace Radical.Windows
{
    [Obsolete("ApplicationBootstrapper has been obsoleted and will be removed in v3.0.0, consider moving to new RadicalApplication using the AddRadicalApplication extension method.", false)]
    public class ApplicationBootstrapper<TMainView> : ApplicationBootstrapper where TMainView : System.Windows.Window
    {
        public ApplicationBootstrapper()
            : base()
        {
            UsingAsShell<TMainView>();
        }
    }
}
