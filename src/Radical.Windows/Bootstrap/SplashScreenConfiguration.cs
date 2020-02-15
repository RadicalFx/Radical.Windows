using Radical.Windows.Presentation;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Radical.Windows.Bootstrap
{
    /// <summary>
    /// The configuration of the SplashScreen.
    /// </summary>
    public class SplashScreenConfiguration
    {
        /// <summary>
        /// SplashScreenConfiguration default constructor.
        /// </summary>
        public SplashScreenConfiguration()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeToContent = SizeToContent.WidthAndHeight;
            MinWidth = 485;
            MinHeight = 335;
            WindowStyle = WindowStyle.None;
            MinimumDelay = 1500;
            SplashScreenViewType = typeof( SplashScreenView );

            StartupAsyncWork = obj => Task.Delay( MinimumDelay );
        }

        /// <summary>
        /// Determines the way the splash screen hosting window is dimensioned, the default value is <c>WidthAndHeight</c>.
        /// </summary>
        public SizeToContent SizeToContent { get; set; }
        
        /// <summary>
        /// The splash screen startup location, the default value is <c>CenterScreen</c>.
        /// </summary>
        public WindowStartupLocation WindowStartupLocation { get; set; }
        
        /// <summary>
        /// The splash screen window style, the default value is <c>None</c>.
        /// </summary>
        public WindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Defines the work that should be executed asynchronously while the splash screen is running.
        /// </summary>
        public Func<IServiceProvider, Task> StartupAsyncWork { get; set; }

        /// <summary>
        /// Defines the Height of the splash screen window if the SizeToContent value is Manual or Width; otherwise is ignored.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Defines the Width of the splash screen window if the SizeToContent value is Manual or Height; otherwise is ignored.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Represents the minimum time, in milliseconds, the splash screen will be shown.
        /// </summary>
        public int MinimumDelay { get; set; }

        /// <summary>
        /// Defines the default view that Radical use to host the splash screen content.
        /// </summary>
        public Type SplashScreenViewType { get; set; }

        /// <summary>
        /// The Minimum Width of the splash screen window. The default value is 585.
        /// </summary>
        public double? MinWidth { get; set; }

        /// <summary>
        /// The Minimum Height of the splash screen window. The default value is 335.
        /// </summary>
        public double? MinHeight { get; set; }
    }
}
