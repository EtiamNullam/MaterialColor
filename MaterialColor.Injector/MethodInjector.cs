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

        /// <summary>
        /// TODO: Improve error handling
        /// </summary>
        /// <param name="passArgumentsByRef">
        /// Doesn't work on calling object
        /// </param>
        public void InjectAsFirst(string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName,
            bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(_sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(_targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(_targetModule, targetTypeName, targetMethodName);
            var targetMethodBody = targetMethod.Body;

            var firstInstruction = targetMethodBody.Instructions.First();

            if (includeCallingObject)
            {
                IncludeCallingObject(firstInstruction, targetMethodBody);
            }

            var methodILProcessor = targetMethodBody.GetILProcessor();

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
            var targetMethodBody = CecilHelper.GetMethodDefinition(_targetModule, targetTypeName, targetMethodName).Body;
            var instruction = targetMethodBody.Instructions[instructionIndex];

            InjectBefore(sourceTypeName, sourceMethodName, targetMethodBody, instruction);
        }

        public void InjectBefore(string sourceTypeName, string sourceMethodName, MethodBody targetMethodBody, Instruction targetInstruction, bool includeCallingObject = false)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(_sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(_targetModule, sourceMethod);

            InjectBefore(sourceMethodReference, targetMethodBody, targetInstruction, includeCallingObject);
        }

        public void InjectBefore(MethodReference sourceMethodReference, MethodBody targetMethodBody, Instruction targetInstruction, bool includeCallingObject = false)
        {
            var methodILProcessor = targetMethodBody.GetILProcessor();

            if (includeCallingObject)
            {
                IncludeCallingObject(targetInstruction, targetMethodBody);
            }

            methodILProcessor.InsertBefore(targetInstruction, Instruction.Create(OpCodes.Call, sourceMethodReference));
        }

        private void IncludeCallingObject(Instruction nextInstruction, MethodBody methodBody)
        {
            var thisInstruction = Instruction.Create(OpCodes.Ldarg_0);
            methodBody.GetILProcessor().InsertBefore(nextInstruction, thisInstruction);
        }
    }
}
