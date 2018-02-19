using System;

namespace Starship.Core.Utility {
    public abstract class Disposable : IDisposable {

        public void Dispose() {
            if (!IsDisposed) {
                IsDisposed = true;
                Disposed();
            }
        }

        public abstract void Disposed();

        private bool IsDisposed { get; set; }
    }
}