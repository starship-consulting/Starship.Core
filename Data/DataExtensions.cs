using System;
using System.Reflection;
using Starship.Core.Data.Attributes;
using Starship.Core.Extensions;

namespace Starship.Core.Data {
    public static class DataExtensions {

        public static PropertyInfo GetPrimaryKeyProperty(this Type type) {
            var property = type.GetPropertyWithAttribute<PrimaryKeyAttribute>();

            if (property == null) {
                property = type.GetProperty("RowKey");
            }

            return property;
        }

        public static string GetPrimaryKey(this object source) {
            var property = source.GetType().GetPrimaryKeyProperty();

            if (property != null) {
                var id = property.GetValue(source);

                if (id != null) {
                    return id.ToString();
                }
            }

            return string.Empty;
        }

        public static PropertyInfo GetPartitionProperty(this Type type) {
            var property = type.GetPropertyWithAttribute<PartitionKeyAttribute>();

            if(property == null) {
                property = type.GetProperty("PartitionKey");
            }

            return property;
        }

        public static string GetPartition(this object source) {
            var property = source.GetType().GetPartitionProperty();

            if (property != null) {
                var partition = property.GetValue(source);

                if (partition != null) {
                    return partition.ToString();
                }
            }

            return string.Empty;
        }
    }
}