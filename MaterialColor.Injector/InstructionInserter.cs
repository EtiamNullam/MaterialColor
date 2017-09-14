using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialColor.Injector
{
    public class InstructionInserter
    {
        public InstructionInserter(MethodDefinition targetMethod) : this(targetMethod.Body.GetILProcessor()) { }

        public InstructionInserter(MethodBody targetMethodBody) : this(targetMethodBody.GetILProcessor()) { }

        public InstructionInserter(ILProcessor targetMethodILProcessor)
        {
            _ilProcessor = targetMethodILProcessor;
        }

        private ILProcessor _ilProcessor;

        public void InsertBefore(Instruction targetInstruction, IEnumerable<Instruction> instructions)
        {
            foreach (var newInstruction in instructions)
            {
                _ilProcessor.InsertBefore(targetInstruction, newInstruction);
            }
        }

        public void InsertBefore(Instruction targetInstruction, Instruction instruction)
        {
            _ilProcessor.InsertBefore(targetInstruction, instruction);
        }

        public void InsertAfter(Instruction targetInstruction, IEnumerable<Instruction> instructions)
        {
            var reversedInstructions = instructions.Reverse();

            foreach (var newInstruction in reversedInstructions)
            {
                _ilProcessor.InsertAfter(targetInstruction, newInstruction);
            }
        }

        public void InsertAfter(int instructionIndex, Instruction instruction)
        {
            InsertAfter(_ilProcessor.Body.Instructions[instructionIndex], instruction);
        }

        public void InsertAfter(Instruction targetInstruction, Instruction instruction)
        {
            _ilProcessor.InsertAfter(targetInstruction, instruction);
        }
    }
}
