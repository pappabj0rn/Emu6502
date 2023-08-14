namespace Emu6502.Instructions;

public abstract class SBC : Instruction
{
    protected void SubtractMemoryWithBorrowFromAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var op1 = cpu.Registers.A;
        var op2 = (byte)~cpu.FetchMemory(addr);

        var result = (ushort)(op1
            + op2
            + (cpu.Flags.C ? 1 : 0));

        var op1Positive = (op1 & 0x80) == 0x00;
        var op2Positive = (op2 & 0x80) == 0x00;

        cpu.Registers.A = (byte)result;
        cpu.Flags.N = (cpu.Registers.A & 0x80) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;
        cpu.Flags.C = result > 0xff;               
        cpu.Flags.V = ((op1Positive && op2Positive) || (!op1Positive && !op2Positive))
                      && cpu.Flags.N == op2Positive;
    }
}

public class SBC_Immediate : SBC
{
    public SBC_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu)
        };
    }
}

public class SBC_Absolute : SBC
{
    public SBC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_AbsoluteX : SBC
{
    public SBC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_AbsoluteY : SBC
{
    public SBC_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_Zeropage : SBC
{
    public SBC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_ZeropageX : SBC
{
    public SBC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_IndirectX : SBC
{
    public SBC_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_IndirectY : SBC
{
    public SBC_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}
