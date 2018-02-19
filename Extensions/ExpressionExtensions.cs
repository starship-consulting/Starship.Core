using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Starship.Core.Extensions {
    public static class ExpressionExtensions {
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