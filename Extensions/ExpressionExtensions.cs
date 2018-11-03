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