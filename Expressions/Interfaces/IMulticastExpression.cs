namespace Starship.Core.Expressions.Interfaces {
    public interface IMulticastExpression {
        object Invoke(object input);
    }
}