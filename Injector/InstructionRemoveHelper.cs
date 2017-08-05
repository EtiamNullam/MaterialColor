using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injector
{
    public static class InstructionRemoveHelper
    {
        public static ModuleDefinition RemoveInstructionAt(string moduleName, string typeName, string methodName, int instructionIndex)
        {
            var module = CecilHelper.GetModule(moduleName);

            return RemoveInstructionAt(module, typeName, methodName, instructionIndex);
        }

        public static ModuleDefinition RemoveInstructionAt(ModuleDefinition module, string typeName, string methodName, int instructionIndex)
        {
            var method = CecilHelper.GetMethodDefinition(module, typeName, methodName);
            var methodBody = method.Body;

            methodBody.GetILProcessor().Remove(methodBody.Instructions[instructionIndex]);

            return module;
        }
    }
}
