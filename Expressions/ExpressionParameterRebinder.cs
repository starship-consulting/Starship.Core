using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Starship.Core.Expressions {
    public class ExpressionParameterRebinder : ExpressionVisitor {
        
        public ExpressionParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map) {
            Map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp) {
            return new ExpressionParameterRebinder(map).Visit(exp);
        }
        
        protected override Expression VisitParameter(ParameterExpression p) {
            ParameterExpression replacement;

            if (Map.TryGetValue(p, out replacement)) {
                p = replacement;
            }

            return base.VisitParameter(p);
        }

        private readonly Dictionary<ParameterExpression, ParameterExpression> Map;
    }
}