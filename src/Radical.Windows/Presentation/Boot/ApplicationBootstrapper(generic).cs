using Microsoft.Extensions.DependencyInjection;

namespace Radical.Windows.Presentation.Boot
{
    public class ApplicationBootstrapper<TMainView> : ApplicationBootstrapper
    {
        public ApplicationBootstrapper()
            : base()
        {

        }

        public ApplicationBootstrapper(IServiceCollection services)
            : base(services)
        {

        }
    }
}
