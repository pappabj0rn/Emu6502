using Emu6502.Instructions;

namespace Emu6502;

public class ExecutionState
{
    public int RemainingCycles { get; set; }
    public int Ticks { get; private set; }

    public Instruction? Instruction { get; set; }
    public int InstructionSubstate { get; set; }

    internal bool Halted => RemainingCycles == 0;

    public void Tick()
    {
        RemainingCycles--;
        Ticks++;
    }

    public void ClearInstruction()
    {
        Instruction = null;
        InstructionSubstate = 0;
    }
}
