namespace Emu6502.Instructions;

public abstract class EOR : Instruction
{
    protected void ExclusiveOrMemoryWithAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var op1 = cpu.Registers.A;
        var op2 = cpu.FetchMemory(addr);

        var result = (byte)(op1 ^ op2);

        cpu.SetRegister(Register.A, result);
    }
}

public class EOR_Immediate : EOR
{
    public EOR_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => ExclusiveOrMemoryWithAccumulator(cpu)
        };
    }
}

public class EOR_Absolute : EOR
{
    public EOR_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}

public class EOR_AbsoluteX : EOR
{
    public EOR_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}

public class EOR_AbsoluteY : EOR
{
    public EOR_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}

public class EOR_Zeropage : EOR
{
    public EOR_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}

public class EOR_ZeropageX : EOR
{
    public EOR_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}

public class EOR_IndirectX : EOR
{
    public EOR_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}

public class EOR_IndirectY : EOR
{
    public EOR_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => ExclusiveOrMemoryWithAccumulator(cpu, Addr));
    }
}
