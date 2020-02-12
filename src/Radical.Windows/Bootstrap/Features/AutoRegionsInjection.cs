using Microsoft.Extensions.DependencyInjection;
using Radical.Windows.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Windows.Bootstrap.Features
{
    class AutoRegionsInjection : IFeature
    {
        readonly Dictionary<string, List<Type>> buffer = new Dictionary<string, List<Type>>();

        public void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings)
        {
            var injectionHandler = serviceProvider.GetRequiredService<IRegionInjectionHandler>();
            foreach (var kvp in buffer)
            {
                injectionHandler.RegisterViewsAsInterestedIn(
                    regionName: kvp.Key,
                    views: kvp.Value.AsEnumerable());
            }

            buffer.Clear();
        }

        internal void Add(string regionName, Type viewType)
        {
            if (buffer.ContainsKey(regionName))
            {
                buffer[regionName].Add(viewType);
            }
            else
            {
                buffer.Add(regionName, new List<Type>() { viewType });
            }
        }
    }
}
