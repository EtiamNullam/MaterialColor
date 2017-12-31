using Mono.Cecil;

namespace Injector
{
    public class Publisher
    {
        public Publisher(ModuleDefinition targetModule)
        {
            _targetModule = targetModule;
        }

        readonly ModuleDefinition _targetModule;

        public void MakeFieldPublic(string typeName, string fieldName)
        {
            var field = CecilHelper.GetFieldDefinition(_targetModule, typeName, fieldName);

            field.IsPublic = true;
        }

        public void MakeMethodPublic( string typeName, string methodName)
        {
            var method = CecilHelper.GetMethodDefinition(_targetModule, typeName, methodName);

            method.IsPublic = true;
        }
    }
}
