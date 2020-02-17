using Microsoft.Extensions.Hosting;
using System.Windows;

namespace RadicalAppWithGenericHost
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var host = new HostBuilder()
                .AddRadicalApplication<Presentation.MainView>()
                .Build();

            Startup += async (s, e) =>
            {
                await host.StartAsync();
            };

            Exit += async (s, e) =>
            {
                using (host)
                {
                    await host?.StopAsync();
                }
            };
        }
    }
}
