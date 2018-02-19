using System;
using System.CodeDom;
using System.Collections.Generic;

namespace Starship.Core.CodeGeneration {
    public class GeneratedClassContext {
        public GeneratedClassContext() {
            Imports = new List<CodeNamespaceImport>();
        }

        public GeneratedClassContext(string className/*, Type baseType*/)
            : this() {
            Constructor = new CodeConstructor();
            Constructor.Attributes = MemberAttributes.Public;

            Type = new CodeTypeDeclaration(className);
            //Type.BaseTypes.Add(new CodeTypeReference(baseType));

            //Imports.Add(new CodeNamespaceImport(baseType.Namespace));

            Type.Members.Add(Constructor);
        }


        public CodeConstructor Constructor { get; set; }

        public CodeTypeDeclaration Type { get; set; }

        public List<CodeNamespaceImport> Imports { get; set; }

        public void AddMember(CodeTypeMember field) {
            Type.Members.Add(field);
        }
    }
}