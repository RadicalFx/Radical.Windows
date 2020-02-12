using System;
using System.Collections.Generic;

namespace Radical.Windows.Services
{
    class ResourcesRegistrationHolder
    {
        public IDictionary<Type, HashSet<Type>> Registrations { get; set; }
    }
}