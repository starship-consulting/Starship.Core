using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Starship.Core.Extensions;

namespace Starship.Core.Reflection {
  public static class DynamicQuery {
    static DynamicQuery() {
      RuntimeTypes = new Dictionary<Type, Type>();
    }

    public static IQueryable SelectByInterface<T>(this IQueryable<T> source, Type interfaceType, bool includeTypeName = true) {
      var properties = interfaceType.GetPublicProperties().Where(each => each.PropertyType.IsSimple()).ToList();

      var objectFields = properties.Select(each => each.Name).ToList();

      if (includeTypeName) {
        objectFields.Add("_Type");
      }

      var sourceType = source.GetType().GetGenericArguments().First();

      var param = Expression.Parameter(sourceType);
      var expressions = new List<MemberBinding>();

      lock (RuntimeTypes) {
        if (!RuntimeTypes.ContainsKey(interfaceType)) {
          var runtimeType = new RuntimeObjectBuilder().CreateNewObject(interfaceType, interfaceType, objectFields.ToArray()).GetType();
          RuntimeTypes.Add(interfaceType, runtimeType);
        }

        var instance = Activator.CreateInstance(RuntimeTypes[interfaceType]);
        var type = instance.GetType();

        if (includeTypeName) {
          var typeProperty = type.GetProperty("_Type");
          expressions.Add(Expression.Bind(typeProperty, Expression.Constant(typeof (T).Name)));
        }

        foreach (var property in instance.GetType().GetProperties()) {
          if (property.Name.StartsWith("_")) {
            continue;
          }

          expressions.Add(Expression.Bind(property, Expression.PropertyOrField(param, property.Name)));
        }

        var init = Expression.MemberInit(Expression.New(instance.GetType()), expressions.ToArray());

        var func = typeof (Func<,>).MakeGenericType(typeof (T), type);
        var lambdaMethods = typeof (Expression).GetMethods().Where(each => each.Name == "Lambda");

        var expression = lambdaMethods.First().MakeGenericMethod(func).Invoke(null, new object[] {init, new[] {param}}) as Expression;

        //var expression = Expression.Lambda<Func<T, I>>(init, param);
        var methods = typeof (Queryable).GetMethods().Where(each => each.Name == "Select").ToList();
        var method = methods.FirstOrDefault().MakeGenericMethod(typeof (T), type);

        return method.Invoke(null, new object[] {source, expression}) as IQueryable;
      }
    }

    public static IQueryable<BaseRuntimeObject> DynamicSelect<T, G>(this IQueryable<IGrouping<G, T>> source, string groupName, params string[] propertyNames) {
      var objectFields = new List<string>(propertyNames);

      objectFields.Add(groupName);

      var sourceType = source.GetType().GetGenericArguments().First();

      var param = Expression.Parameter(sourceType);
      var expressions = new List<MemberBinding>();
      var instance = new RuntimeObjectBuilder().CreateNewObject(typeof (T), null, objectFields.ToArray());
      var type = instance.GetType();
      var groupProperty = type.GetProperty(groupName);

      foreach (var propertyName in propertyNames) {
        var property = type.GetProperty(propertyName);

        var sumMethod = typeof (Enumerable).GetMethods()
          .FirstOrDefault(x => x.Name == "Sum" && x.ContainsGenericParameters && x.ReturnType == property.PropertyType);

        if (sumMethod != null) {
          var eachEntity = Expression.Parameter(typeof (T), "each");
          var propertyAccess = Expression.Property(eachEntity, propertyName);

          var lambda = Expression.Lambda(propertyAccess, eachEntity);

          var sumExpression = Expression.Call(sumMethod.MakeGenericMethod(typeof (T)), param, lambda);

          expressions.Add(Expression.Bind(property, sumExpression));
        }
        else {
          expressions.Add(Expression.Bind(property, Expression.PropertyOrField(param, propertyName)));
        }
      }

      expressions.Add(Expression.Bind(groupProperty, Expression.PropertyOrField(param, "Key")));

      var init = Expression.MemberInit(Expression.New(instance.GetType()), expressions.ToArray());

      var expression = Expression.Lambda<Func<IGrouping<G, T>, BaseRuntimeObject>>(init, param);

      return source.Select(expression);
    }

    /*private static IQueryable<T> InternalDynamicSelect<T>(this IQueryable source, string groupName, params string[] propertyNames)
    {
        var objectFields = new List<string>(propertyNames);

        objectFields.Add(groupName);

        var sourceType = source.GetType().GetGenericArguments().First();

        var param = Expression.Parameter(sourceType);
        var expressions = new List<MemberBinding>();
        var instance = new RuntimeObjectBuilder().CreateNewObject<T>(objectFields.ToArray());
        var type = instance.GetType();
        var groupProperty = type.GetProperty(groupName);

        foreach (var propertyName in propertyNames)
        {
            var property = type.GetProperty(propertyName);

            var sumMethod = typeof(Enumerable).GetMethods()
                .FirstOrDefault(x => x.Name == "Sum" && x.ContainsGenericParameters && x.ReturnType == property.PropertyType);

            if (sumMethod != null)
            {
                var eachEntity = Expression.Parameter(typeof(T), "each");
                var propertyAccess = Expression.Property(eachEntity, propertyName);

                var lambda = Expression.Lambda(propertyAccess, eachEntity);

                var sumExpression = Expression.Call(sumMethod.MakeGenericMethod(typeof(T)), param, lambda);

                expressions.Add(Expression.Bind(property, sumExpression));
            }
            else
            {
                expressions.Add(Expression.Bind(property, Expression.PropertyOrField(param, propertyName)));
            }
        }

        expressions.Add(Expression.Bind(groupProperty, Expression.PropertyOrField(param, "Key")));

        var init = Expression.MemberInit(Expression.New(instance.GetType()), expressions.ToArray());

        var expression = Expression.Lambda<Func<IGrouping<Guid, T>, BaseRuntimeObject>>(init, param);

        //return expression;
        //return (IQueryable<T>)source.InvokeMethod("Select", expression);
        return source.AsQueryable().Select(expression);
    }*/

    /*public static Expression<Func<IGrouping<Guid, T>, BaseRuntimeObject>> GetSelectExpression<T>(string groupName, params string[] propertyNames)
    {
        var objectFields = new List<string>(propertyNames);

        objectFields.Add(groupName);

        // Todo:  Create IGrouping typeof at runtime
        var param = Expression.Parameter(typeof(IGrouping<Guid, T>));
        var expressions = new List<MemberBinding>();
        var instance = new RuntimeObjectBuilder().CreateNewObject<T>(objectFields.ToArray());
        var type = instance.GetType();
        var groupProperty = type.GetProperty(groupName);

        foreach (var propertyName in propertyNames)
        {
            var property = type.GetProperty(propertyName);

            var sumMethod = typeof(Enumerable).GetMethods()
                .FirstOrDefault(x => x.Name == "Sum" && x.ContainsGenericParameters && x.ReturnType == property.PropertyType);

            if (sumMethod != null)
            {
                var eachEntity = Expression.Parameter(typeof(T), "each");
                var propertyAccess = Expression.Property(eachEntity, propertyName);

                var lambda = Expression.Lambda(propertyAccess, eachEntity);

                var sumExpression = Expression.Call(sumMethod.MakeGenericMethod(typeof(T)), param, lambda);

                expressions.Add(Expression.Bind(property, sumExpression));
            }
            else
            {
                expressions.Add(Expression.Bind(property, Expression.PropertyOrField(param, propertyName)));
            }
        }

        expressions.Add(Expression.Bind(groupProperty, Expression.PropertyOrField(param, "Key")));

        var init = Expression.MemberInit(Expression.New(instance.GetType()), expressions.ToArray());
        return Expression.Lambda<Func<IGrouping<Guid, T>, BaseRuntimeObject>>(init, param);
    }*/

    /*private Expression<Func<T, BaseRuntimeObject>> BuildSelect(params string[] propertyNames)
    {
        var param = Expression.Parameter(typeof(T), "value");
        var expressions = new List<MemberBinding>();
        var instance = new RuntimeObjectBuilder().CreateNewObject<T>(propertyNames);

        foreach (var propertyName in propertyNames)
        {
            var property = instance.GetType().GetProperty(propertyName);

            if (property.PropertyType == typeof(decimal))
            {
            }

            expressions.Add(Expression.Bind(property, Expression.PropertyOrField(param, propertyName)));
        }

        var init = Expression.MemberInit(Expression.New(instance.GetType()), expressions.ToArray());
        return Expression.Lambda<Func<T, BaseRuntimeObject>>(init, param);
    }*/
    
    public static Expression GenerateSelector<TEntity>(ParameterExpression parameter, string propertyName, out Type resultType) where TEntity : class {
      var property = typeof (TEntity).GetProperty(propertyName);
      var propertyAccess = Expression.MakeMemberAccess(parameter, property);
      resultType = property.PropertyType;
      return propertyAccess;
    }

    public static Expression GetPropertyExpression(Type type, string property) {
      var value = Expression.Parameter(type, "value");
      var valueProperty = Expression.PropertyOrField(value, property);
      var lambda = Expression.Lambda(valueProperty, value);
      return lambda;
    }

    public static Expression<Func<T, R>> GetPropertyExpression<T, R>(string property) {
      var value = Expression.Parameter(typeof (T), "value");
      var valueProperty = Expression.PropertyOrField(value, property);
      var lambda = Expression.Lambda<Func<T, R>>(valueProperty, value);

      return lambda;
    }

    public static Dictionary<Type, Type> RuntimeTypes { get; set; }
  }
}