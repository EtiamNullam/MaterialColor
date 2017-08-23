using Mono.Cecil;
using System.Linq;

namespace MaterialColor.Injector
{
    public static class InstructionRemoveHelper
    {
        public static void RemoveInstructionAt(ModuleDefinition module, string typeName, string methodName, int instructionIndex)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);
            var methodBody = method.Body;

            methodBody.GetILProcessor().Remove(methodBody.Instructions[instructionIndex]);
        }

        public static void ClearAllButLast(ModuleDefinition module, string typeName, string methodName)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);
            var methodBody = method.Body;

            var methodILProcessor = methodBody.GetILProcessor();

            for (int i = methodBody.Instructions.Count-1; i > 0; i--)
            {
                methodILProcessor.Remove(methodBody.Instructions.First());
            }
        }
    }
}
