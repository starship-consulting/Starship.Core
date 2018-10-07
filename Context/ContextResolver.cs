using System;
using System.Collections.Generic;
using Starship.Core.Extensions;
using Starship.Core.Interfaces;

namespace Starship.Core.Context {
    public abstract class ContextResolver {

        protected abstract void Clear(string contextKey);

        protected abstract string Get(string contextKey);

        protected abstract void Set(string contextKey, string value);

        public void Detach(IsContext target, string contextKey) {
            lock(Contexts) {
                Clear(contextKey);
                
                var key = target.GetId();

                if (Contexts.ContainsKey(key)) {
                    Contexts.Remove(key);
                }
            }
        }

        public HasId Get(Type type, string contextKey) {
            lock (Contexts) {
                var id = Get(contextKey);
                return string.IsNullOrEmpty(id) ? New(type, contextKey) : Contexts[id];
            }
        }

        public T Get<T>(string contextKey) where T : HasId, new() {
            return (T)Get(typeof(T), contextKey);
        }

        protected virtual IsContext InitializeContext(Type type) {
            return type.New<IsContext>();
        }

        private IsContext New(Type type, string contextKey) {
            lock(Contexts) {
                /*var existingKey = Get(contextKey);

                if (!string.IsNullOrEmpty(existingKey)) {
                    Detach(Contexts[existingKey], contextKey);
                }*/

                var context = type.New<IsContext>();
                var id = context.GetId();
                Set(contextKey, id);
                Contexts.Add(id, context);
                return context;
            }
        }

        private readonly Dictionary<string, IsContext> Contexts = new Dictionary<string, IsContext>();
    }
}