using System;
using System.Collections.Generic;
using Starship.Core.Expressions;

namespace Starship.Core.Rules {
    public class RuleDefinition {
        public RuleDefinition() {
        }

        public RuleDefinition(KeyValuePair<Type, IsRule> rule) {
            Type = rule.Key.Name;
            Description = ExpressionReader.ConvertExpressionToText(rule.Value.GetExpression());
        }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}