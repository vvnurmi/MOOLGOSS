using ImpromptuInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public interface IEntity
    {
    }

    public static class EntityExtensions
    {
        public static T GetValue<T>(this IEntity entity, Property<T> prop)
        {
            var property = entity.GetProperties()
                .FirstOrDefault(p => p.Name == prop.Name && p.PropertyType == prop.Type);
            if (property == null) throw new PropertyException();
            return (T)property.GetValue(entity);
        }

        public static IEnumerable<IProp> GetProps(this IEntity entity)
        {
            return entity.GetProperties()
                .Select(p =>
                {
                    var propertyType = typeof(Property<>).MakeGenericType(p.PropertyType);
                    return (IProp)Activator.CreateInstance(propertyType, p.Name);
                });
        }

        private static IEnumerable<PropertyInfo> GetProperties(this IEntity entity)
        {
            return entity.GetType().GetProperties()
                .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(PropAttribute)));
        }
    }
}
