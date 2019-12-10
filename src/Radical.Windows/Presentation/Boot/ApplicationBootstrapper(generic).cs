﻿using Microsoft.Extensions.DependencyInjection;

namespace Radical.Windows.Presentation.Boot
{
    public class ApplicationBootstrapper<TMainView> : ApplicationBootstrapper where TMainView : System.Windows.Window
    {
        public ApplicationBootstrapper()
            : base()
        {
            UsingAsShell<TMainView>();
        }

        public ApplicationBootstrapper(IServiceCollection services)
            : base(services)
        {
            UsingAsShell<TMainView>();
        }
    }
}
