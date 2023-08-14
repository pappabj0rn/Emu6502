namespace Emu6502.Instructions;

public abstract class LDX : Instruction
{
    protected void LoadMemoryIntoX(
        ICpu cpu,
        ushort? addr = null)
    {
        cpu.Registers.X = cpu.FetchMemory(addr);
        cpu.Flags.N = (cpu.Registers.X & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.X == 0;
    }
}

public class LDX_Immediate : LDX
{
    public LDX_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => LoadMemoryIntoX(cpu)
        };
    }
}

public class LDX_Zeropage : LDX
{
    public LDX_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoX(cpu, Addr));
    }
}

public class LDX_ZeropageY : LDX
{
    public LDX_ZeropageY()
    {
        SubTasks = ZeropageYAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoX(cpu, Addr));
    }
}

public class LDX_Absolute : LDX
{
    public LDX_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoX(cpu, Addr));
    }
}

public class LDX_AbsoluteY : LDX
{
    public LDX_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoX(cpu, Addr));
    }
}
