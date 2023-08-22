namespace Emu6502.Instructions;

public abstract class BIT : Instruction
{
    protected void BitOperation(ICpu cpu)
    {
        var value = cpu.FetchMemory(Addr);
        var res = (byte)(cpu.Registers.A & value);

        cpu.Flags.N = (value & 0x80) > 1;
        cpu.Flags.V = (value & 0x40) > 1;
        cpu.Flags.Z = res == 0x00;
    }
}

public class BIT_Zeropage : BIT
{
    public BIT_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add(BitOperation);
    }
}

public class BIT_Absolute : BIT
{
    public BIT_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add(BitOperation);
    }
}