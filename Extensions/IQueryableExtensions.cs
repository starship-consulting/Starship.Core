using System;
using System.Linq;
using System.Linq.Expressions;
using Starship.Core.Expressions;

namespace Starship.Core.Extensions {
    public static class IQueryableExtensions {

        public static object IsomorphicFirstOrDefault(this IQueryable query) {
            return typeof(Queryable).GetMethods()
                .First(each => each.Name == "FirstOrDefault" && each.GetParameters().Count() == 1)
                .MakeGenericMethod(query.GetGenericType()).Invoke(null, new object[] { query });
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