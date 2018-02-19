using System.Linq.Expressions;

namespace Starship.Core.Expressions {
    public class ExpressionWrapper<T> {
        public ExpressionWrapper() {
        }

        public ExpressionWrapper(Expression<T> expression) {
            SetExpression(expression);
        }

        protected void SetExpression(Expression<T> expression) {
            Expression = expression;
            CompiledExpression = Expression.Compile();
        }

        protected Expression<T> Expression { get; set; }

        protected T CompiledExpression { get; set; }
    }
}