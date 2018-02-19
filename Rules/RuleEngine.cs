using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Starship.Core.Rules {
    public class RuleEngine {
        public RuleEngine() {
            Rules = new Dictionary<Type, IsRule>();
        }

        public Rule<T> AddRule<T>(Expression<Func<T, bool>> ruleExpression) {
            var rule = new Rule<T>(ruleExpression);
            Rules.Add(typeof (T), rule);
            return rule;
        }

        public void AddRule<T>(T rule) where T : IsRule {
            Rules.Add(typeof (T), rule);
        }

        public Rule<T> GetViolatingRule<T>(object context) {
            return GetViolatingRule(context) as Rule<T>;
        }

        public IsRule GetViolatingRule(object context) {
            var rule = GetRule(context.GetType());

            if (rule != null && !rule.Validate(context)) {
                return rule;
            }

            return null;
        }

        public List<RuleDefinition> GetDefinitions() {
            return Rules.Select(rule => new RuleDefinition(rule)).ToList();
        }

        private IsRule GetRule(Type type) {
            return Rules.ContainsKey(type) ? Rules[type] : null;
        }

        private Dictionary<Type, IsRule> Rules { get; set; }
    }
}