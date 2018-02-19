using System;
using System.Collections.Generic;
using System.Linq;
using Starship.Core.Interfaces;

namespace Starship.Core.Utility {
    public class AttributeProcessor {

        public void Process(object instance) {
            var type = instance.GetType();
            var classAttributes = type.GetCustomAttributes(typeof (IObjectProcessor), true).Cast<IObjectProcessor>().ToList();

            Process(instance, classAttributes);

            foreach (var property in type.GetProperties()) {
                var attributes = property.GetCustomAttributes(typeof(IObjectProcessor), true).Cast<IObjectProcessor>().ToList();
                Process(instance, attributes);
            }
        }

        private void Process(object value, IEnumerable<IObjectProcessor> processors) {
            foreach (var attribute in processors) {
                attribute.Process(value);
            }
        }
    }
}