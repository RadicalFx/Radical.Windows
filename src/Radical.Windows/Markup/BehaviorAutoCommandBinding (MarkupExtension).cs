﻿using Radical.Windows.Behaviors;
using Radical.Windows.CommandBuilders;
using Radical.Windows.ComponentModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace Radical.Windows.Markup
{
    public class BehaviorAutoCommandBinding : AutoCommandBinding
    {
        protected override void OnProvideValue(IServiceProvider provider, object value)
        {

            if (TryGetTargetItems(provider, out DependencyObject fe, out DependencyProperty dp))
            {
                if (fe is INotifyAttachedOjectLoaded inab)
                {
                    void h(object s, EventArgs e)
                    {
                        inab.AttachedObjectLoaded -= h;
                        OnTargetLoaded(fe, dp);
                    }

                    inab.AttachedObjectLoaded += h;
                }
            }
        }

        protected override void SetInputBindings(DependencyObject target, ICommandSource source, IDelegateCommand command)
        {
            //Not supported ?
        }

        protected override DelegateCommandBuilder GetCommandBuilder()
        {
            return new BehaviorDelegateCommandBuilder();
        }
    }
}