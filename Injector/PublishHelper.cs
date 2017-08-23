using Mono.Cecil;

namespace MaterialColor.Injector
{
    public static class PublishHelper
    {
        public static void MakeFieldPublic(ModuleDefinition module, string typeName, string fieldName)
        {
            var field = CecilHelper.GetFieldDefinition(module, typeName, fieldName);

            field.IsPublic = true;
        }

        public static void MakeMethodPublic(ModuleDefinition module, string typeName, string methodName)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);

            method.IsPublic = true;
        }
    }
}
