namespace Emu6502.Instructions;

public abstract class ADC : Instruction
{
    protected void AddMemoryAndCarryToAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var result = (ushort)(cpu.FetchMemory(addr)
            + cpu.Registers.A
            + (cpu.Flags.C ? 1 : 0));

        cpu.Registers.A = (byte)(result & 0xff);
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;
        cpu.Flags.C = result > 0xff;        
    }
}

public class ADC_Immediate : ADC
{
    public ADC_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => AddMemoryAndCarryToAccumulator(cpu)
        };
    }
}
