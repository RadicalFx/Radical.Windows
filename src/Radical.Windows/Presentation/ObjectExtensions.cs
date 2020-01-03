using System.Reflection;

namespace Radical.Windows.Presentation
{
    static class ObjectExtensions
    {
        public static PropertyInfo GetProperty(this object entity, string propertyName) 
        {
            return entity.GetType().GetProperty(propertyName);
        }
    }
}
