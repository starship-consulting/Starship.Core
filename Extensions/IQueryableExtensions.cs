using System;
using System.Linq;
using System.Linq.Expressions;
using Starship.Core.Expressions;

namespace Starship.Core.Extensions {
    public static class IQueryableExtensions {
        
        public static object FirstOrDefault(this IQueryable query) {
            var method = typeof(IQueryableExtensions).GetMethods().FirstOrDefault(each => each.Name == "FirstOrDefault" && each.IsGenericMethod);
            var generic = method.MakeGenericMethod(query.ElementType);
            return generic.Invoke(null, new object[] { query });
        }

        public static T FirstOrDefault<T>(IQueryable query) {
            return Queryable.FirstOrDefault(query.Cast<T>());
        }

        public static IQueryable Where(this IQueryable source, string propertyName, object value) {
            var parameter = Expression.Parameter(source.ElementType, "entity");
            var property = Expression.Property(parameter, propertyName);
            var valueExpression = Expression.Constant(value);
            var equals = Expression.Equal(property, valueExpression);

            var whereCallExpression = Expression.Call(typeof (Queryable), "Where", new[] {source.ElementType}, source.Expression, Expression.Lambda(equals, parameter));

            return source.Provider.CreateQuery(whereCallExpression);
        }

        public static IQueryable Where<T>(this IQueryable source, Expression<Func<T, bool>> expression) where T : class {
            var type = source.GetGenericType();
            var convertedExpression = typeof (ExpressionConverter).GetMethod("TypeConvert").MakeGenericMethod(typeof (T), type).Invoke(null, new object[] {expression});
            return (IQueryable) typeof (Queryable).GetMethods().FirstOrDefault(each => each.Name == "Where").MakeGenericMethod(type).Invoke(null, new[] {source, convertedExpression});
        }
    }
}