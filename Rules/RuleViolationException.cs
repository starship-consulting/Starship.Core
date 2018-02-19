using System;
using Starship.Core.Expressions;

namespace Starship.Core.Rules {
    public class RuleViolationException : Exception {
        public RuleViolationException(string message)
            : base(message) {
        }

        public RuleViolationException(IsRule rule)
            : this(ExpressionReader.ConvertExpressionToText(rule.GetExpression()))
        {
        }
    }
}