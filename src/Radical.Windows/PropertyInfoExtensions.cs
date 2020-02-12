using Radical.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Radical.Windows
{
    static class PropertyInfoExtensions
    {
        public static string GetDisplayName(this PropertyInfo propertyInfo) 
        {
            if (propertyInfo != null && propertyInfo.IsAttributeDefined<DisplayAttribute>())
            {
                var a = propertyInfo.GetAttribute<DisplayAttribute>();
                return a.GetName();
            }

            if (propertyInfo != null && propertyInfo.IsAttributeDefined<DisplayNameAttribute>())
            {
                var a = propertyInfo.GetAttribute<DisplayNameAttribute>();
                return a.DisplayName;
            }

            return null;
        }
    }
}
