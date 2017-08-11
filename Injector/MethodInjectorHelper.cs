using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Injector
{
    public static class MethodInjectorHelper
    {
        public static ModuleDefinition InjectAsFirstInstructionAtEntryPoint(string sourceModuleName, string targetModuleName, string sourceTypeName, string sourceMethodName)
        {
            var sourceModule = CecilHelper.GetModule(sourceModuleName);
            var targetModule = CecilHelper.GetModule(targetModuleName);

            var sourceMethod = CecilHelper.GetMethodDefinition(sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(targetModule, sourceMethod);

            var entryPointBody = targetModule.EntryPoint.Body;
            var firstInstruction = entryPointBody.Instructions.First();

            entryPointBody.GetILProcessor().InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, sourceMethodReference));

            return targetModule;
        }

        public static ModuleDefinition InjectAsFirstInstruction(string sourceModuleName, string targetModuleName, string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName)
        {
            var sourceModule = CecilHelper.GetModule(sourceModuleName);
            var targetModule = CecilHelper.GetModule(targetModuleName);

            return InjectAsFirstInstruction(sourceModule, targetModule, sourceTypeName, sourceMethodName, targetTypeName, targetMethodName);
        }

        public static ModuleDefinition InjectAsFirstInstruction(ModuleDefinition sourceModule, ModuleDefinition targetModule, string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName, bool includeCallingObject = false, int includeArgumentCount = 0)
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

            return targetModule;
        }

        public static ModuleDefinition InjectBefore(ModuleDefinition sourceModule, ModuleDefinition targetModule, string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName, int instructionIndex)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(targetModule, targetTypeName, targetMethodName);
            var targetMethodBody = targetMethod.Body;

            var instruction = targetMethodBody.Instructions[instructionIndex];

            targetMethodBody.GetILProcessor().InsertBefore(instruction, Instruction.Create(OpCodes.Call, sourceMethodReference));

            return targetModule;
        }
    }
}
