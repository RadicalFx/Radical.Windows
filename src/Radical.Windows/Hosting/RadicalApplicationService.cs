using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Windows.Hosting
{
    class RadicalApplicationService : IHostedService
    {
        IServiceProvider serviceProvider;

        public RadicalApplicationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            //var viewResolver = serviceProvider.GetService<IViewResolver>();
            //var mainView = viewResolver.GetView<Presentation.MainView>();

            //var app = Application.Current;
            //app.MainWindow = new Presentation.MainView();
            //app.MainWindow.Show();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
