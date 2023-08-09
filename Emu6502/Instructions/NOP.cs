namespace Emu6502.Instructions;

public class NOP : Instruction
{
    public override void Execute(ICpu cpu)
    {
        cpu.State.Tick();
        cpu.State.ClearInstruction();
    }
}