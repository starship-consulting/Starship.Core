using System;
using System.Linq.Expressions;
using Starship.Core.FluentApi;

namespace Starship.Core.Rules {
    public class Rule<T> : IsRule {
        public Rule() {
        }

        public Rule(string violationMessage) {
            ViolationMessage = violationMessage;
        }

        public Rule(Expression<Func<T, Func<bool>>> expression) {
            RuleExpression = new MulticastExpression<T, bool>(expression);
        }

        public Rule(Expression<Func<T, bool>> expression) {
            RuleExpression = new MulticastExpression<T, bool>(expression);
        }

        public Rule(Expression<Func<T, Func<bool>>> expression, string violationMessage) : this(violationMessage) {
            RuleExpression = new MulticastExpression<T, bool>(expression);
        }

        public Rule(Expression<Func<T, bool>> expression, string violationMessage) : this(violationMessage) {
            RuleExpression = new MulticastExpression<T, bool>(expression);
        }

        /*public Rule(Expression<Func<T, Func<C, bool>>> expression, string violationMessage)
            : this(violationMessage)
        {
            RuleExpression = new BooleanExpressionAdapter<T>(expression);
        }*/

        public bool Validate(object context) {
            return RuleExpression.Invoke((T)context);
        }

        public Expression GetExpression() {
            return RuleExpression.GetExpression();
        }

        public string ViolationMessage { get; set; }

        private MulticastExpression<T, bool> RuleExpression { get; set; }
    }
}