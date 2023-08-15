namespace Emu6502.Instructions;

public abstract class ORA : Instruction
{
    protected void OrMemoryWithAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var op1 = cpu.Registers.A;
        var op2 = cpu.FetchMemory(addr);

        var result = (ushort)(op1 | op2);

        cpu.Registers.A = (byte)(result & 0xff);
        cpu.Flags.N = (cpu.Registers.A & 0x80) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;
    }
}

public class ORA_Immediate : ORA
{
    public ORA_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => OrMemoryWithAccumulator(cpu)
        };
    }
}

public class ORA_Absolute : ORA
{
    public ORA_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}

public class ORA_AbsoluteX : ORA
{
    public ORA_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}

public class ORA_AbsoluteY : ORA
{
    public ORA_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}

public class ORA_Zeropage : ORA
{
    public ORA_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}

public class ORA_ZeropageX : ORA
{
    public ORA_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}

public class ORA_IndirectX : ORA
{
    public ORA_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}

public class ORA_IndirectY : ORA
{
    public ORA_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => OrMemoryWithAccumulator(cpu, Addr));
    }
}