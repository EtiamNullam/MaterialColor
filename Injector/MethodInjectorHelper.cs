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

        public static ModuleDefinition InjectAsFirstInstruction(ModuleDefinition sourceModule, ModuleDefinition targetModule, string sourceTypeName, string sourceMethodName, string targetTypeName, string targetMethodName)
        {
            var sourceMethod = CecilHelper.GetMethodDefinition(sourceModule, sourceTypeName, sourceMethodName);
            var sourceMethodReference = CecilHelper.GetMethodReference(targetModule, sourceMethod);

            var targetMethod = CecilHelper.GetMethodDefinition(targetModule, targetTypeName, targetMethodName);
            var targetMethodBody = targetMethod.Body;

            var firstInstruction = targetMethodBody.Instructions.First();

            targetMethodBody.GetILProcessor().InsertBefore(firstInstruction, Instruction.Create(OpCodes.Call, sourceMethodReference));

            return targetModule;
        }
    }
}
