using System;
using System.Collections.Generic;

namespace Radical.Windows
{
    class ResourcesRegistrationHolder
    {
        public IDictionary<Type, HashSet<Type>> Registrations { get; } = new Dictionary<Type, HashSet<Type>>();
    }
}