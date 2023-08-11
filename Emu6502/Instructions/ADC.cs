namespace Emu6502.Instructions;

//TODO Cpu.Flags.V not handled
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

public class ADC_Absolute : ADC
{
    public ADC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_AbsoluteX : ADC
{
    public ADC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_AbsoluteY : ADC
{
    public ADC_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_Zeropage : ADC
{
    public ADC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_ZeropageX : ADC
{
    public ADC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_IndirectX : ADC
{
    public ADC_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_IndirectY : ADC
{
    public ADC_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}