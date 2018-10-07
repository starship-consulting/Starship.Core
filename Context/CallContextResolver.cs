using System;
using System.Runtime.Remoting.Messaging;

namespace Starship.Core.Context {
    public class CallContextResolver : ContextResolver {

        protected override void Clear(string contextKey) {
            CallContext.FreeNamedDataSlot(contextKey);
        }

        protected override string Get(string contextKey) {
            return CallContext.LogicalGetData(contextKey) as string;
        }

        protected override void Set(string contextKey, string value) {
            CallContext.LogicalSetData(contextKey, value);
        }
    }
}