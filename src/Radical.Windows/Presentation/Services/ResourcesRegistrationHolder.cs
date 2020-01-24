using System;
using System.Collections.Generic;

namespace Radical.Windows.Presentation.Services
{
    class ResourcesRegistrationHolder
    {
        public IDictionary<Type, HashSet<Type>> Registrations { get; set; }
    }
}