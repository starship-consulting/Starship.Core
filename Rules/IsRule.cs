using System.Linq.Expressions;

namespace Starship.Core.Rules {
    public interface IsRule {
        bool Validate(object context);
        Expression GetExpression();
    }
}