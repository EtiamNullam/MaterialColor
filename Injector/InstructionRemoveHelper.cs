using Mono.Cecil;
using System.Linq;

namespace MaterialColor.Injector
{
    public static class InstructionRemoveHelper
    {
        public static ModuleDefinition RemoveInstructionAt(ModuleDefinition module, string typeName, string methodName, int instructionIndex)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);
            var methodBody = method.Body;

            methodBody.GetILProcessor().Remove(methodBody.Instructions[instructionIndex]);

            return module;
        }

        public static ModuleDefinition ClearAllButLast(ModuleDefinition module, string typeName, string methodName)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);
            var methodBody = method.Body;

            var methodILProcessor = methodBody.GetILProcessor();

            for (int i = methodBody.Instructions.Count-1; i > 0; i--)
            {
                methodILProcessor.Remove(methodBody.Instructions.First());
            }

            return module;
        }
    }
}
