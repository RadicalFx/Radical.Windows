﻿using System;
using System.Security.Principal;
using System.Threading;

namespace Radical.Windows.Bootstrap.Features
{
    class CurrentPrincipal : IFeature
    {
        public void Setup(IServiceProvider serviceProvider, BootstrapConfiguration bootstrapConfiguration)
        {
            Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        }
    }
}
