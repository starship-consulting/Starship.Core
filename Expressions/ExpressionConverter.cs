using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Starship.Core.Expressions {
    public static class ExpressionConverter {
        public static Expression<Func<TTo, bool>> TypeConvert<TFrom, TTo>(this Expression<Func<TFrom, bool>> from) {
            if (from == null) return null;

            return ConvertImpl<Func<TFrom, bool>, Func<TTo, bool>>(from);
        }

        private static Expression<TTo> ConvertImpl<TFrom, TTo>(Expression<TFrom> from)
            where TFrom : class
            where TTo : class {

            var fromTypes = from.Type.GetGenericArguments();
            var toTypes = typeof (TTo).GetGenericArguments();

            if (fromTypes.Length != toTypes.Length)
                throw new NotSupportedException("Incompatible lambda function-type signatures");

            Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();
            for (int i = 0; i < fromTypes.Length; i++) {
                if (fromTypes[i] != toTypes[i])
                    typeMap[fromTypes[i]] = toTypes[i];
            }

            // re-map all parameters that involve different types
            Dictionary<Expression, Expression> parameterMap = new Dictionary<Expression, Expression>();
            ParameterExpression[] newParams = GenerateParameterMap<TFrom>(from, typeMap, parameterMap);

            // rebuild the lambda
            var body = new TypeConversionVisitor<TTo>(parameterMap).Visit(from.Body);
            return Expression.Lambda<TTo>(body, newParams);
        }

        private static ParameterExpression[] GenerateParameterMap<TFrom>(
            Expression<TFrom> from,
            Dictionary<Type, Type> typeMap,
            Dictionary<Expression, Expression> parameterMap
            )
            where TFrom : class {
            var newParams = new ParameterExpression[from.Parameters.Count];

            for (int i = 0; i < newParams.Length; i++) {
                Type newType;
                if (typeMap.TryGetValue(from.Parameters[i].Type, out newType)) {
                    parameterMap[from.Parameters[i]] = newParams[i] = Expression.Parameter(newType, from.Parameters[i].Name);
                }
            }
            return newParams;
        }

        private class TypeConversionVisitor<T> : ExpressionVisitor {
            private readonly Dictionary<Expression, Expression> parameterMap;

            public TypeConversionVisitor(Dictionary<Expression, Expression> parameterMap) {
                this.parameterMap = parameterMap;
            }

            protected override Expression VisitParameter(ParameterExpression node) {
                // re-map the parameter
                Expression found;
                if (!parameterMap.TryGetValue(node, out found))
                    found = base.VisitParameter(node);
                return found;
            }

            public override Expression Visit(Expression node) {
                LambdaExpression lambda = node as LambdaExpression;
                if (lambda != null && !parameterMap.ContainsKey(lambda.Parameters.First())) {
                    return new TypeConversionVisitor<T>(parameterMap).Visit(lambda.Body);
                }
                return base.Visit(node);
            }

            protected override Expression VisitMember(MemberExpression node) {
                // re-perform any member-binding
                var expr = Visit(node.Expression);
                if (expr.Type != node.Type) {
                    if (expr.Type.GetMember(node.Member.Name).Any()) {
                        MemberInfo newMember = expr.Type.GetMember(node.Member.Name).Single();
                        return Expression.MakeMemberAccess(expr, newMember);
                    }
                }
                return base.VisitMember(node);
            }
        }

        public static Expression<Func<TTo, bool>> Convert<TFrom, TTo>(Expression<Func<TFrom, bool>> expression) {
            Dictionary<Expression, Expression> substitutues = new Dictionary<Expression, Expression>();
            var oldParam = expression.Parameters[0];
            var newParam = Expression.Parameter(typeof (TTo), oldParam.Name);
            substitutues.Add(oldParam, newParam);
            Expression body = ConvertNode(expression.Body, substitutues);
            return Expression.Lambda<Func<TTo, bool>>(body, newParam);
        }

        private static Expression ConvertNode(Expression node, IDictionary<Expression, Expression> subst) {
            if (node == null) return null;
            if (subst.ContainsKey(node)) return subst[node];

            switch (node.NodeType) {
                case ExpressionType.Constant:
                    return node;
                case ExpressionType.MemberAccess: {
                    var me = (MemberExpression) node;
                    var newNode = ConvertNode(me.Expression, subst);
                    return Expression.MakeMemberAccess(newNode, newNode.Type.GetMember(me.Member.Name).Single());
                }
                case ExpressionType.Equal: {
                    var be = (BinaryExpression) node;
                    return Expression.MakeBinary(be.NodeType, ConvertNode(be.Left, subst), ConvertNode(be.Right, subst), be.IsLiftedToNull, be.Method);
                }
                default:
                    throw new NotSupportedException(node.NodeType.ToString());
            }
        }
    }
}