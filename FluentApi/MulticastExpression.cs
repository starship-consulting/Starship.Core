using System;
using System.Linq.Expressions;
using Starship.Core.Expressions.Interfaces;

namespace Starship.Core.FluentApi {
    public class MulticastExpression<T, R> : IMulticastExpression {
        public MulticastExpression(MulticastDelegate del) {
            Delegate = del;
        }

        public MulticastExpression(Expression<Func<T, R>> expression) : this(expression.Compile()) {
            Expression = expression;
        }

        public MulticastExpression(Expression<Func<T, Func<R>>> expression) : this(expression.Compile()) {
            Expression = expression;
        }

        public object Invoke(object input) {
            return RecursiveInvoke(input, Delegate);
        }

        public R Invoke(T parameter) {
            return RecursiveInvoke(parameter, Delegate);
        }

        private R RecursiveInvoke(object parameter, MulticastDelegate del) {
            var parameters = del.Method.GetParameters().Length;
            var result = parameters > 0 ? del.DynamicInvoke(parameter) : del.DynamicInvoke(null);

            if (result is MulticastDelegate) {
                return RecursiveInvoke(parameter, result as MulticastDelegate);
            }

            return (R) result;
        }

        public Expression GetExpression() {
            return Expression;
        }

        public MulticastDelegate Delegate { get; set; }

        private Expression Expression { get; set; }
    }
}