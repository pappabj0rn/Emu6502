namespace Emu6502.Instructions;

public class NOP : Instruction
{
    public NOP()
    {
        SubTasks = new()
        {
            (cpu) => { cpu.State.Tick(); }
        };
    }
}