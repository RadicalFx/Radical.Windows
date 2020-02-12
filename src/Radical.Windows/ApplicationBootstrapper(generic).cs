namespace Radical.Windows
{
    public class ApplicationBootstrapper<TMainView> : ApplicationBootstrapper where TMainView : System.Windows.Window
    {
        public ApplicationBootstrapper()
            : base()
        {
            UsingAsShell<TMainView>();
        }
    }
}
