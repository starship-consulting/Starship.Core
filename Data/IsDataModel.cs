using System.Linq;
using Starship.Core.Interfaces;

namespace Starship.Core.Data {
    public interface IsDataModel : HasId {
    }

    public interface IsDataModel<T> : IsDataModel, HasIdentity<T> {
    }

    public interface IsDataModel<in IN, out OUT> : IsDataModel {
        IQueryable<OUT> Get(IQueryable<IN> query);
    }
}