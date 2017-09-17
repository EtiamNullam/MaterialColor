using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace MaterialColor.Injector
{
    public class InstructionRemover
    {
        public InstructionRemover(ModuleDefinition targetModule)
        {
            _targetModule = targetModule;
        }

        ModuleDefinition _targetModule;

        public void ReplaceByNopAt(string typeName, string methodName, int instructionIndex)
        {
            var method = CecilHelper.GetMethodDefinition(_targetModule, typeName, methodName);
            var methodBody = method.Body;

            ReplaceByNop(methodBody, methodBody.Instructions[instructionIndex]);
        }

        public void ReplaceByNop(MethodBody methodBody, Instruction instruction)
        {
            methodBody.GetILProcessor().Replace(instruction, Instruction.Create(OpCodes.Nop));
        }

        public void ClearAllButLast(string typeName, string methodName)
        {
            var method = CecilHelper.GetMethodDefinition(_targetModule, typeName, methodName);
            var methodBody = method.Body;

            var methodILProcessor = methodBody.GetILProcessor();

            for (int i = methodBody.Instructions.Count - 1; i > 0; i--)
            {
                methodILProcessor.Remove(methodBody.Instructions.First());
            }
        }
    }
}
