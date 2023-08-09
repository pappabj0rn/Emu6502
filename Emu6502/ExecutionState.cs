using Emu6502.Instructions;

namespace Emu6502;

internal class ExecutionState
{
    internal int RemainingCycles;
    internal int Ticks;
    
    internal Instruction? Instruction;
    internal int InstructionSubstate;

    internal bool Halted => RemainingCycles == 0;

    internal void Tick()
    {
        RemainingCycles--;
        Ticks++;
    }

    internal void ClearInstruction()
    {
        Instruction = null;
        InstructionSubstate = 0;
    }
}
