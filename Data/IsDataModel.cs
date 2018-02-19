using Starship.Core.Interfaces;

namespace Starship.Core.Data {
    public interface IsDataModel {
        string GetId();
    }

    public interface IsDataModel<T> : IsDataModel, HasIdentity<T> {
    }
}