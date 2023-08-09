namespace Emu6502.Instructions;

public class LDA_Immediate : Instruction
{
    public override void Execute(ICpu cpu)
    {
        cpu.Registers.A = cpu.FetchMemory();
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;

        cpu.State.ClearInstruction();
    }
}

public class LDA_Absolute : Instruction
{
    private ushort _addr = 0;

    public LDA_Absolute()
    {
        SubTasks = new() { 
            (cpu) => _addr = cpu.FetchMemory(),
            (cpu) => _addr += (ushort)(cpu.FetchMemory() << 8),
            (cpu) => {
                cpu.Registers.A = cpu.FetchMemory(_addr);
                cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
                cpu.Flags.Z = cpu.Registers.A == 0;
            }
        };
    }
}
