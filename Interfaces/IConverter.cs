using System;
using Starship.Core.Reflection;

namespace Starship.Core.Interfaces {
    public interface IConverter {
        bool CanConvert(Type type, object instance);
        object Convert(MethodInvoker invoker, Type type, object instance);
    }
}