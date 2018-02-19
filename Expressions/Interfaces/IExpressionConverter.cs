using System.Threading.Tasks;

namespace Starship.Core.Expressions.Interfaces {
    public interface IExpressionConverter {
        Task<object> ConvertAsync(object input);
        object Convert(object input);
    }
}