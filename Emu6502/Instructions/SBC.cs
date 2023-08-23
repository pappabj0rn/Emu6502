namespace Emu6502.Instructions;

public abstract class SBC : ADC
{
    protected override byte FetchOp2(ICpu cpu, ushort? addr = null)
    {
        return (byte)~base.FetchOp2(cpu, addr);
    }
}

public class SBC_Immediate : SBC
{
    public SBC_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => AddMemoryAndCarryToAccumulator(cpu)
        };
    }
}

public class SBC_Absolute : SBC
{
    public SBC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class SBC_AbsoluteX : SBC
{
    public SBC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class SBC_AbsoluteY : SBC
{
    public SBC_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class SBC_Zeropage : SBC
{
    public SBC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class SBC_ZeropageX : SBC
{
    public SBC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class SBC_IndirectX : SBC
{
    public SBC_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class SBC_IndirectY : SBC
{
    public SBC_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}
