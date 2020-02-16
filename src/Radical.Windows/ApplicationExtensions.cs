using Radical.Windows;
using Radical.Windows.Bootstrap;

namespace System.Windows
{
    public static class ApplicationExtensions
    {
        public static RadicalApplication AddRadicalApplication(this Application application, Action<BootstrapConfiguration> configure) 
        {
            var configuration = new BootstrapConfiguration();
            configure(configuration);

            return RadicalApplication.BoundTo(application, configuration);
        }
    }
}
