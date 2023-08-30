namespace Emu6502.Instructions;

public abstract class Compare : Instruction
{
    protected abstract byte ComparedRegister(ICpu cpu);

    protected void CompareAWithOpAndSetFlags(ICpu cpu, byte op)
    {
        var result = ComparedRegister(cpu) + (((byte)~op) + 1);

        cpu.Flags.N = (result & 0x80) > 0;
        cpu.Flags.Z = ((byte)result) == 0;
        cpu.Flags.C = result > 0xFF;
    }
}

public abstract class CMP : Compare
{
    protected override byte ComparedRegister(ICpu cpu)
    {
        return cpu.Registers.A;
    }
}

public class CMP_Immediate : CMP
{
    public CMP_Immediate()
    {
        SubTasks = new()
        {
            (cpu) =>
            {
                CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory());
            }
        };
    }
}

public class CMP_Zeropage : CMP
{
    public CMP_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CMP_ZeropageX : CMP
{
    public CMP_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CMP_Absolute : CMP
{
    public CMP_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CMP_AbsoluteX : CMP
{
    public CMP_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CMP_AbsoluteY : CMP
{
    public CMP_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CMP_IndirectX : CMP
{
    public CMP_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CMP_IndirectY : CMP
{
    public CMP_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public abstract class CPX : Compare
{
    protected override byte ComparedRegister(ICpu cpu)
    {
        return cpu.Registers.X;
    }
}

public class CPX_Immediate : CPX
{
    public CPX_Immediate()
    {
        SubTasks = new()
        {
            (cpu) =>
            {
                CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory());
            }
        };
    }
}

public class CPX_Zeropage : CPX
{
    public CPX_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CPX_Absolute : CPX
{
    public CPX_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public abstract class CPY : Compare
{
    protected override byte ComparedRegister(ICpu cpu)
    {
        return cpu.Registers.Y;
    }
}

public class CPY_Immediate : CPY
{
    public CPY_Immediate()
    {
        SubTasks = new()
        {
            (cpu) =>
            {
                CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory());
            }
        };
    }
}

public class CPY_Zeropage : CPY
{
    public CPY_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}

public class CPY_Absolute : CPY
{
    public CPY_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => { CompareAWithOpAndSetFlags(cpu, cpu.FetchMemory(Addr)); });
    }
}