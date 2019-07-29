using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Starship.Core.Expressions;

namespace Starship.Core.Extensions {
    public static class ExpressionExtensions {

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge) {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ExpressionParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName) {

            var entityType = typeof(T);
            var propertyInfo = entityType.GetProperty("Item");
            var arg = Expression.Parameter(entityType, "x");
            //var property = Expression.Property(arg, propertyName);
            var property = Expression.MakeIndex(arg, propertyInfo, new [] { Expression.Constant(propertyName) });
            var selector = Expression.Lambda(property, arg);
            var enumarableType = typeof(Queryable);
            var method = enumarableType.GetMethods()
                .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
                .Where(m => {
                    var parameters = m.GetParameters().ToList();        
                    return parameters.Count == 2;
                }).Single();

            MethodInfo genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);
            
            return (IOrderedQueryable<T>)genericMethod.Invoke(genericMethod, new object[] { query, selector });
        }
        
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) {
            return first.Compose(second, Expression.And);
        }
        
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second) {
            return first.Compose(second, Expression.Or);
        }

        public static MemberExpression Access<T>(this ParameterExpression parameter, Expression<Func<T, object>> expression) {
            return Expression.MakeMemberAccess(parameter, GetMember(expression));
        }

        public static MemberInfo GetMember<T, A>(this Expression<Func<T, A>> expression) {
            var lambda = expression as LambdaExpression;
            var unary = lambda.Body as UnaryExpression;

            if (unary != null) {
                var member = unary.Operand as MemberExpression;
                return member.Member;
            }

            var property = lambda.Body as MemberExpression;
            return property.Member;
        }
    }
}