using System;
using System.Linq.Expressions;

namespace Starship.Core.FluentApi {
    public interface IBooleanExpressionAdapter {
        bool Invoke(object parameter);
        Expression GetExpression();
    }

    public class BooleanExpressionAdapter<T> : IBooleanExpressionAdapter  {
        
        public BooleanExpressionAdapter(Expression<Func<T, Func<bool>>> expression) {
            Expression = new MulticastExpression<T, bool>(expression);
        }

        public BooleanExpressionAdapter(Expression<Func<T, bool>> expression) {
            Expression = new MulticastExpression<T, bool>(expression);
        }

        public bool Invoke(object parameter) {
            return Expression.Invoke((T)parameter);
        }

        public Expression GetExpression() {
            return Expression.GetExpression();
        }

        private MulticastExpression<T, bool> Expression { get; set; }
    }
}