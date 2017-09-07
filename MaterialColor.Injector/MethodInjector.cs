using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MaterialColor.Injector
{
    public class MethodInjector
    {
        public MethodInjector(ModuleDefinition sourceModule, ModuleDefinition targetModule)
        {
            _sourceModule = sourceModule;
            _targetModule = targetModule;
        }

        private ModuleDefinition _sourceModule;
        private ModuleDefinition _targetModule;

        /// <param name="passArgumentsByRef">
        /// Doesn't work on calling object
        /// </param>
        public void InjectAsFirstInstruction(string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName, bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(_sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(_targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(_targetModule, targetTypeName, targetMethodName);
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
                var argumentOpCode = passArgumentsByRef ? OpCodes.Ldarga : OpCodes.Ldarg;

                for (int i = includeArgumentCount; i > 0; i--)
                {
                    var argumentInstruction = Instruction.Create(argumentOpCode, targetMethod.Parameters[i-1]);
                    methodILProcessor.InsertBefore(firstInstruction, argumentInstruction);
                }
            }

            var targetInstruction = Instruction.Create(OpCodes.Call, sourceMethodReference);
            methodILProcessor.InsertBefore(firstInstruction, targetInstruction);
        }

        public void InjectBefore(string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName, int instructionIndex)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(_sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(_targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(_targetModule, targetTypeName, targetMethodName);
            var targetMethodBody = targetMethod.Body;

            var instruction = targetMethodBody.Instructions[instructionIndex];

            targetMethodBody.GetILProcessor().InsertBefore(instruction, Instruction.Create(OpCodes.Call, sourceMethodReference));
        }
    }
}
