namespace Emu6502.Instructions;

public abstract class AND : Instruction
{
    protected void AndMemoryWithAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var op1 = cpu.Registers.A;
        var op2 = cpu.FetchMemory(addr);

        var result = (byte)(op1 & op2);

        cpu.SetRegister(Register.A, result);
    }
}

public class AND_Immediate : AND
{
    public AND_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => AndMemoryWithAccumulator(cpu)
        };
    }
}

public class AND_Absolute : AND
{
    public AND_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}

public class AND_AbsoluteX : AND
{
    public AND_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}

public class AND_AbsoluteY : AND
{
    public AND_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}

public class AND_Zeropage : AND
{
    public AND_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}

public class AND_ZeropageX : AND
{
    public AND_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}

public class AND_IndirectX : AND
{
    public AND_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}

public class AND_IndirectY : AND
{
    public AND_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => AndMemoryWithAccumulator(cpu, Addr));
    }
}
