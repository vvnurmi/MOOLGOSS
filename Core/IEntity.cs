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
        public static object GetValue(this IEntity entity, Prop prop)
        {
            var property = entity.GetProperties()
                .FirstOrDefault(p => p.Name == prop.Name && p.PropertyType == prop.Type);
            if (property == null) throw new PropertyException();
            return property.GetValue(entity);
        }

        public static T GetValue<T>(this IEntity entity, Prop prop)
        {
            return (T)entity.GetValue(prop);
        }

        public static IEnumerable<Prop> GetProps(this IEntity entity)
        {
            return entity.GetProperties().Select(p => new Prop(p.Name, p.PropertyType));
        }

        private static IEnumerable<PropertyInfo> GetProperties(this IEntity entity)
        {
            return entity.GetType().GetProperties()
                .Where(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(PropAttribute)));
        }
    }
}
