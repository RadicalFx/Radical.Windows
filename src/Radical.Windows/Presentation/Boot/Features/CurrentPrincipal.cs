using System;
using System.Security.Principal;
using System.Threading;

namespace Radical.Windows.Presentation.Boot.Features
{
    class CurrentPrincipal : IFeature
    {
        public void Setup(IServiceProvider serviceProvider)
        {
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        }
    }
}
