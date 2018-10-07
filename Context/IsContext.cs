using System;
using Starship.Core.Interfaces;

namespace Starship.Core.Context {
    public interface IsContext : IDisposable, HasId {
    }
}