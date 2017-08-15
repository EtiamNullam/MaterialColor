using Mono.Cecil;

namespace Injector
{
    public static class PublishHelper
    {
        public static ModuleDefinition MakeFieldPublic(ModuleDefinition module, string typeName, string fieldName)
        {
            var field = CecilHelper.GetFieldDefinition(module, typeName, fieldName);

            field.IsPublic = true;

            return module;
        }

        public static ModuleDefinition MakeMethodPublic(ModuleDefinition module, string typeName, string methodName)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);

            method.IsPublic = true;

            return module;
        }
    }
}
