using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MaterialColor.Injector
{
    public static class MethodInjectorHelper
    {
        public static void InjectAsFirstInstruction(ModuleDefinition sourceModule, ModuleDefinition targetModule, string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName, bool includeCallingObject = false, int includeArgumentCount = 0)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(targetModule, targetTypeName, targetMethodName);
            var targetMethodBody = targetMethod.Body;

            var firstInstruction = targetMethodBody.Instructions.First();
            var methodILProcessor = targetMethodBody.GetILProcessor();

            if (includeCallingObject)
            {
                var thisInstruction = Instruction.Create(OpCodes.Ldarg_0);
                methodILProcessor.InsertBefore(firstInstruction, thisInstruction);
            }

            if (includeArgumentCount > 0)
            {
                for (int i = includeArgumentCount; i > 0; i--)
                {
                    var argumentInstruction = Instruction.Create(OpCodes.Ldarg, targetMethod.Parameters[i-1]);
                    methodILProcessor.InsertBefore(firstInstruction, argumentInstruction);
                }
            }

            var targetInstruction = Instruction.Create(OpCodes.Call, sourceMethodReference);
            methodILProcessor.InsertBefore(firstInstruction, targetInstruction);
        }

        public static void InjectBefore(ModuleDefinition sourceModule, ModuleDefinition targetModule, string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName, int instructionIndex)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(targetModule, targetTypeName, targetMethodName);
            var targetMethodBody = targetMethod.Body;

            var instruction = targetMethodBody.Instructions[instructionIndex];

            targetMethodBody.GetILProcessor().InsertBefore(instruction, Instruction.Create(OpCodes.Call, sourceMethodReference));
        }
    }
}
