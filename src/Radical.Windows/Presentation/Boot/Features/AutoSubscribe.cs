using Microsoft.Extensions.DependencyInjection;
using Radical.ComponentModel.Messaging;
using Radical.Linq;
using Radical.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Radical.Windows.Presentation.Boot.Features
{
    class AutoSubscribe : IFeature
    {
        readonly List<Entry> entries = new List<Entry>();

        public void Setup(IServiceProvider serviceProvider, ApplicationSettings applicationSettings)
        {
            var broker = serviceProvider.GetRequiredService<IMessageBroker>();
            foreach (var entry in entries)
            {
                entry.Subscribe(broker, serviceProvider);
            }
        }

        internal void Add(Type implementation, IEnumerable<Type> contracts)
        {
            entries.Add(new Entry() { Implementation = implementation });
        }
    }

    class Entry
    {
        public Type Implementation { get; set; }

        public void Subscribe(IMessageBroker broker, IServiceProvider serviceProvider)
        {
            var invocationModel = Implementation.Is<INeedSafeSubscription>() ?
                    InvocationModel.Safe :
                    InvocationModel.Default;

            Implementation.GetInterfaces()
                .Where(i => i.Is<IHandleMessage>() && i.IsGenericType)
                .ForEach(genericHandler =>
                {
                    var messageType = genericHandler.GetGenericArguments().Single();
                    broker.Subscribe(this, messageType, invocationModel, (s, msg) =>
                    {
                        var handler = serviceProvider.GetService(Implementation) as IHandleMessage;

                        if (handler.ShouldHandle(s, msg))
                        {
                            handler.Handle(s, msg);
                        }
                    });
                });
        }
    }
}
