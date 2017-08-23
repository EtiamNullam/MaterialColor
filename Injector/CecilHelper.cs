using Mono.Cecil;
using System.Linq;

namespace MaterialColor.Injector
{
    public static class CecilHelper
    {
        public static ModuleDefinition GetModule(string moduleName, string directoryPath)
        {
            var parameters = new ReaderParameters();
            var assemblyResolver = new DefaultAssemblyResolver();

            assemblyResolver.AddSearchDirectory(directoryPath);
            parameters.AssemblyResolver = assemblyResolver;

            return ModuleDefinition.ReadModule(moduleName, parameters);
        }

        public static MethodReference GetMethodReference(ModuleDefinition targetModule, MethodDefinition method)
        {
            return targetModule.Import(method);
        }

        public static MethodDefinition GetMethodDefinition(ModuleDefinition module, string typeName, string methodName)
        {
            var type = GetTypeDefinition(module, typeName);

            return GetMethodDefinition(module, type, methodName);
        }

        public static MethodDefinition GetMethodDefinition(ModuleDefinition module, TypeDefinition type, string methodName)
        {
            return type.Methods.First(method => method.Name == methodName);
        }

        public static FieldDefinition GetFieldDefinition(ModuleDefinition module, string typeName, string fieldName)
        {
            var type = GetTypeDefinition(module, typeName);

            return GetFieldDefinition(type, fieldName);
        }

        public static FieldDefinition GetFieldDefinition(TypeDefinition type, string fieldName)
        {
            return type.Fields.First(field => field.Name == fieldName);
        }

        public static TypeDefinition GetTypeDefinition(ModuleDefinition module, string typeName)
        {
            return module.Types.First(type => type.Name == typeName);
        }
    }
}
