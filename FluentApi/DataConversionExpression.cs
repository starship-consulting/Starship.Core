using System;
using System.Linq.Expressions;

namespace Starship.Core.FluentApi {
    public interface IDataConversionExpression {
        object Invoke(object source);
    }

    public class DataConversionExpression<FROM, TO> : IDataConversionExpression {
        public DataConversionExpression(Expression<Func<FROM, TO>> expression) {
            Expression = expression;
        }

        public object Invoke(object source) {
            return Expression.Compile().Invoke((FROM)source);
        }

        private Expression<Func<FROM, TO>> Expression { get; set; }
    }
}