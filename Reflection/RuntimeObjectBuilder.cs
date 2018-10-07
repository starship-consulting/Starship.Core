using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Starship.Core.Extensions;

namespace Starship.Core.Reflection {
  public class RuntimeObjectBuilder {
    public RuntimeObjectBuilder() {
      Type = null;
    }

    public object CreateNewObject(Type sourceType, Type interfaceType, params string[] propertyNames) {
      var fields = new List<BaseRuntimeObject.Field>();

      foreach (var propertyName in propertyNames) {
        var property = sourceType.GetPublicProperties().FirstOrDefault(each => each.Name == propertyName);

        fields.Add(new BaseRuntimeObject.Field {
          FieldName = propertyName,
          FieldType = property != null ? property.PropertyType : typeof (object)
        });
      }

      return CreateNewObject(interfaceType, fields.ToArray());
    }

    public object CreateNewObject(Type parentType, params BaseRuntimeObject.Field[] fields) {
      Type = CompileResultType(parentType, fields);
      var myObject = Activator.CreateInstance(Type);

      return myObject;
    }

    public IList GetObjectList() {
      Type listType = typeof (List<>).MakeGenericType(Type);

      return (IList) Activator.CreateInstance(listType);
    }

    public static MethodInfo GetCompareToMethod(object genericInstance, string sortExpression) {
      Type genericType = genericInstance.GetType();
      object sortExpressionValue = genericType.GetProperty(sortExpression).GetValue(genericInstance, null);
      Type sortExpressionType = sortExpressionValue.GetType();
      MethodInfo compareToMethodOfSortExpressionType = sortExpressionType.GetMethod("CompareTo", new Type[] {sortExpressionType});

      return compareToMethodOfSortExpressionType;
    }

    public static Type CompileResultType(Type interfaceType, params BaseRuntimeObject.Field[] fields) {
      TypeBuilder tb = GetTypeBuilder(interfaceType);
      tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

      foreach (var field in fields) {
        CreateProperty(tb, field.FieldName, field.FieldType, interfaceType);
      }

      Type objectType = tb.CreateType();
      return objectType;
    }

    private static TypeBuilder GetTypeBuilder(Type interfaceType) {
      var typeSignature = $"{"robject"}_{Guid.NewGuid().ToString("N")}";

      AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(typeSignature), AssemblyBuilderAccess.Run);
      ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("core");
      TypeBuilder tb = moduleBuilder.DefineType(typeSignature
        , TypeAttributes.Public |
          TypeAttributes.Class |
          TypeAttributes.AutoClass |
          TypeAttributes.AnsiClass |
          TypeAttributes.BeforeFieldInit |
          TypeAttributes.AutoLayout
        , null);


      if (interfaceType != null) {
        tb.AddInterfaceImplementation(interfaceType);
      }

      tb.SetParent(typeof (BaseRuntimeObject));

      return tb;
    }

    private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType, Type interfaceType) {
      FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
      var flags = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;

      PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
      MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, flags, propertyType, Type.EmptyTypes);
      ILGenerator getIl = getPropMthdBldr.GetILGenerator();

      getIl.Emit(OpCodes.Ldarg_0);
      getIl.Emit(OpCodes.Ldfld, fieldBuilder);
      getIl.Emit(OpCodes.Ret);

      MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, flags, null, new[] {propertyType});

      ILGenerator setIl = setPropMthdBldr.GetILGenerator();
      var modifyProperty = setIl.DefineLabel();
      var exitSet = setIl.DefineLabel();

      setIl.MarkLabel(modifyProperty);
      setIl.Emit(OpCodes.Ldarg_0);
      setIl.Emit(OpCodes.Ldarg_1);
      setIl.Emit(OpCodes.Stfld, fieldBuilder);

      setIl.Emit(OpCodes.Nop);
      setIl.MarkLabel(exitSet);
      setIl.Emit(OpCodes.Ret);

      propertyBuilder.SetGetMethod(getPropMthdBldr);
      propertyBuilder.SetSetMethod(setPropMthdBldr);

      if (interfaceType != null) {
        var interfaceMethod = interfaceType.GetPublicProperties().FirstOrDefault(each => each.Name == propertyName);

        if (interfaceMethod != null) {
          typeBuilder.DefineMethodOverride(getPropMthdBldr, interfaceMethod.GetGetMethod());
          typeBuilder.DefineMethodOverride(setPropMthdBldr, interfaceMethod.GetSetMethod());
        }
      }
    }

    public Type Type { get; set; }
  }

  public class BaseRuntimeObject {
    public class Field {
      public string FieldName { get; set; }

      public Type FieldType { get; set; }
    }
  }
}