using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Injector
{
    public class MethodInjector
    {
        public MethodInjector(ModuleDefinition sourceModule, ModuleDefinition targetModule)
        {
            _sourceModule = sourceModule;
            _targetModule = targetModule;
        }

        private readonly ModuleDefinition _sourceModule;
        private readonly ModuleDefinition _targetModule;

        /// <param name="passArgumentsByRef">
        /// Doesn't work on calling object
        /// </param>
        public void InjectAsFirst(string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName,
            bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false)
        {
            InjectBefore(sourceTypeName, sourceMethodName, targetTypeName, targetMethodName, 0, includeCallingObject, includeArgumentCount, passArgumentsByRef);
        }

        public void InjectAsFirst(string sourceTypeName, string sourceMethodName, MethodBody targetMethodBody,
            bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false)
        {
            InjectBefore(sourceTypeName, sourceMethodName, targetMethodBody, targetMethodBody.Instructions.First(),
                includeCallingObject, includeArgumentCount, passArgumentsByRef);
        }

        public void InjectBefore(string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName,
            int instructionIndex, bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false, bool useFullName = false)
        {
            var targetMethodBody = CecilHelper.GetMethodDefinition(_targetModule, targetTypeName, targetMethodName, useFullName).Body;
            var instruction = targetMethodBody.Instructions[instructionIndex];

            InjectBefore(sourceTypeName, sourceMethodName, targetMethodBody, instruction, includeCallingObject, includeArgumentCount, passArgumentsByRef, useFullName: useFullName);
        }

        public void InjectBefore(string sourceTypeName, string sourceMethodName, MethodBody targetMethodBody,
            Instruction targetInstruction, bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false, bool useFullName = false)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(_sourceModule, sourceTypeName, sourceMethodName, useFullName: useFullName);
            var sourceMethodReference = CecilHelper.GetMethodReference(_targetModule, sourceMethod);

            InjectBefore(sourceMethodReference, targetMethodBody, targetInstruction, includeCallingObject, includeArgumentCount, passArgumentsByRef);
        }

        public void InjectBefore(MethodReference sourceMethodReference, MethodBody targetMethodBody,
            Instruction targetInstruction, bool includeCallingObject = false, int includeArgumentCount = 0, bool passArgumentsByRef = false)
        {
            var methodILProcessor = targetMethodBody.GetILProcessor();

            if (includeCallingObject)
            {
                IncludeCallingObject(targetInstruction, targetMethodBody);
            }

            if (includeArgumentCount > 0)
            {
                var argumentOpCode = passArgumentsByRef ? OpCodes.Ldarga : OpCodes.Ldarg;

                for (int i = includeArgumentCount; i > 0; i--)
                {
                    var argumentInstruction = Instruction.Create(argumentOpCode, targetMethodBody.Method.Parameters[i-1]);
                    methodILProcessor.InsertBefore(targetInstruction, argumentInstruction);
                }
            }

            methodILProcessor.InsertBefore(targetInstruction, Instruction.Create(OpCodes.Call, sourceMethodReference));
        }

        private MethodBody GetMethodBody(string typeName, string methodName)
            => CecilHelper.GetMethodDefinition(_targetModule, typeName, methodName).Body;

        private void IncludeCallingObject(Instruction nextInstruction, MethodBody methodBody)
        {
            var thisInstruction = Instruction.Create(OpCodes.Ldarg_0);
            methodBody.GetILProcessor().InsertBefore(nextInstruction, thisInstruction);
        }
    }
}
