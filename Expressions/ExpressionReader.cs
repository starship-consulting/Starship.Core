using System.Linq.Expressions;
using Starship.Core.Extensions;

namespace Starship.Core.Expressions {

    public static class ExpressionReader {

        public static string ConvertExpressionToText(Expression expression) {
            var visitor = new ExpressionToTextVisitor();
            return visitor.GetText(expression).Capitalize() + ".";
        }
    }
}