using System;

namespace Radical.Windows
{
    [Obsolete("ApplicationBootstrapper has been obsoleted and will be treated as an error in v3.0.0 and removed in v4.0.0. Consider moving to new RadicalApplication using the AddRadicalApplication extension method.", false)]
    public class ApplicationBootstrapper<TMainView> : ApplicationBootstrapper where TMainView : System.Windows.Window
    {
        public ApplicationBootstrapper()
            : base()
        {
            UsingAsShell<TMainView>();
        }
    }
}
