using System.Windows;

namespace RadicalApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.AddRadicalApplication<Presentation.MainView>();
        }
    }
}
