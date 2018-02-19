using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

namespace Starship.Core.CodeGeneration {
    public class ProxyClientGenerator {
        public ProxyClientGenerator(string outputDirectory, List<Type> types) {
            OutputDirectory = outputDirectory;
            Types = types;
        }

        private string OutputDirectory { get; set; }

        private List<Type> Types { get; set; }

        public void Generate() {
            foreach (Type type in Types) {
                var classContext = new GeneratedClassContext(type.Name);
                //classContext.Constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (RestClient), "client"));
                //classContext.Constructor.BaseConstructorArgs.Add(new CodeSnippetExpression("client"));

                bool validController = false;

                /*foreach (MethodInfo method in type.GetMethods()) {
                    var route = method.GetCustomAttribute<RouteAttribute>();

                    if (route == null) {
                        continue;
                    }

                    classContext.AddMember(GenerateActionProxy(method, route.Template));
                    validController = true;
                }

                if (!validController) {
                    continue;
                }

                controllerTypes.Add(className);

                WriteClassToFile(classContext, "Proxies/");*/
            }
        }
    }

    /*public class RestSharpProxyGenerator {

    public RestSharpProxyGenerator(string outputDirectory) {
      OutputDirectory = outputDirectory;
    }

    public void Generate() {

      var controllerTypes = new List<string>();

      foreach (var type in typeof (BaseApiController).Assembly.GetTypes()) {
        if (typeof(BaseApiController).IsAssignableFrom(type)) {

          var className = type.Name + "Proxy";
          var classContext = new GeneratedClassContext(className, typeof(BaseControllerProxy));
          classContext.Constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof (RestClient), "client"));
          classContext.Constructor.BaseConstructorArgs.Add(new CodeSnippetExpression("client"));

          var validController = false;

          foreach (var method in type.GetMethods()) {

            var route = method.GetCustomAttribute<RouteAttribute>();

            if (route == null) {
              continue;
            }

            classContext.AddMember(GenerateActionProxy(method, route.Template));
            validController = true;
          }

          if (!validController) {
            continue;
          }

          controllerTypes.Add(className);

          WriteClassToFile(classContext, "Proxies/");
        }
      }

      GenerateClient(controllerTypes);
    }

    private void GenerateClient(IEnumerable<string> types) {
      var classContext = new GeneratedClassContext("LogbookApiClient", typeof(BaseLogbookApiClient));
      classContext.Constructor.BaseConstructorArgs.Add(new CodeSnippetExpression(""));

      foreach (var type in types) {
        var fieldType = new CodeTypeReference(type);
        var fieldName = type.Replace("ControllerProxy", "");
        var field = GenerateField(fieldName, fieldType);

        classContext.Constructor.Statements.Add(InstantiateField(field, "Client"));

        classContext.AddMember(field);
      }

      WriteClassToFile(classContext);
    }

    private CodeAssignStatement SetFieldValue(CodeMemberField member, string variableName) {
      var expression = new CodeVariableReferenceExpression(variableName);
      var reference = new CodeThisReferenceExpression();

      return new CodeAssignStatement(new CodePropertyReferenceExpression(reference, member.Name), expression);
    }

    private CodeAssignStatement InstantiateField(CodeMemberField member, string variableName) {

      var expression = new CodeObjectCreateExpression(member.Type, new CodeVariableReferenceExpression(variableName));
      var reference = new CodeThisReferenceExpression();

      return new CodeAssignStatement(new CodePropertyReferenceExpression(reference, member.Name), expression);
    }

    private CodeMemberField GenerateField(string name, CodeTypeReference propertyType) {
      var property = new CodeMemberField();
      property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
      property.Type = propertyType;
      property.Name = name + "";

      return property;
    }

    private CodeMemberMethod GenerateActionProxy(MethodInfo methodInfo, string route) {

      var method = new CodeMemberMethod {
        Name = methodInfo.Name,
        ReturnType = new CodeTypeReference(),
        Attributes = MemberAttributes.Public | MemberAttributes.Final
      };

      var actionMethod = GetActionMethod(methodInfo);

      var parameters = new List<CodeExpression>();
      parameters.Add(new CodePrimitiveExpression(route));
      parameters.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Method)), actionMethod.ToString()));

      method.Statements.Add(InstantiateVariable("request", typeof(RestRequest), parameters.ToArray()));

      foreach (var parameter in methodInfo.GetParameters()) {
        method.Parameters.Add(new CodeParameterDeclarationExpression(parameter.ParameterType, parameter.Name));

        var requestMethodName = "AddParameter";
        CodeExpression keyExpression = new CodePrimitiveExpression(parameter.Name);
        CodeExpression valueExpression = new CodeVariableReferenceExpression(parameter.Name);

        if (route.ToLower().Contains("{" + parameter.Name.ToLower() + "}")) {
          requestMethodName = "AddUrlSegment";
          valueExpression = new CodeVariableReferenceExpression(parameter.Name + ".ToString()");
        }

        method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("request"), requestMethodName, new [] { keyExpression, valueExpression }));
      }

      var resultType = ConvertActionResultType(methodInfo.ReturnType);
      var response = typeof (IRestResponse<>).MakeGenericType(resultType);
      var action = typeof (Action<>).MakeGenericType(response);

      method.Parameters.Add(new CodeParameterDeclarationExpression(action, "response"));

      var executeExpression = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Execute", new CodeVariableReferenceExpression("request"), new CodeVariableReferenceExpression("response"));
      method.Statements.Add(executeExpression);

      return method;
    }

    private CodeStatement InstantiateVariable(string variableName, Type type, params CodeExpression[] parameters) {
      return new CodeVariableDeclarationStatement(type, variableName, new CodeObjectCreateExpression(type, parameters));
    }

    private Type ConvertActionResultType(Type type) {
      var convertedType = type;

      while (convertedType.IsGenericType) {
        convertedType = convertedType.GetGenericArguments().First();
      }

      if (convertedType.IsAssignableFrom(typeof(HttpResponseMessage))) {
        convertedType = typeof(object);
      }

      return convertedType;
    }

    private Method GetActionMethod(MethodInfo method) {

      if (method.GetCustomAttribute<HttpPostAttribute>() != null) {
        return Method.POST;
      }

      if (method.GetCustomAttribute<HttpPutAttribute>() != null) {
        return Method.PUT;
      }

      if (method.GetCustomAttribute<HttpPostAttribute>() != null) {
        return Method.POST;
      }

      if (method.GetCustomAttribute<HttpDeleteAttribute>() != null) {
        return Method.DELETE;
      }

      switch (method.Name.ToLower()) {
        case "post":
          return Method.POST;
        case "put":
          return Method.PUT;
        case "delete":
          return Method.DELETE;
        default:
          return Method.GET;
      }
    }

    private void WriteClassToFile(GeneratedClassContext classContext, string subFolder = "") {

      var provider = new CSharpCodeProvider();
      var unit = new CodeCompileUnit();

      var proxyNamespace = new CodeNamespace("Logbook");

      foreach (var import in classContext.Imports) {
        proxyNamespace.Imports.Add(import);
      }

      unit.Namespaces.Add(proxyNamespace);
      proxyNamespace.Types.Add(classContext.Type);

      var path = OutputDirectory + "/" + subFolder + classContext.Type.Name + ".cs";

      Directory.CreateDirectory(Path.GetDirectoryName(path));

      using (var writer = new StreamWriter(path, false)) {
        using (var indent = new IndentedTextWriter(writer, "  ")) {
          provider.GenerateCodeFromCompileUnit(unit, indent, new CodeGeneratorOptions());
        }
      }
    }

    private string OutputDirectory { get; set; }
  }*/
}