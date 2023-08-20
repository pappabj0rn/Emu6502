namespace Emu6502.Instructions;

public class STX : Instruction
{
    protected void WriteXToMemoryAtAddr(ICpu cpu)
    {
        cpu.WriteMemory(cpu.Registers.X, Addr);
    }
}

public class STX_Zeropage : STX
{
    public STX_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add(WriteXToMemoryAtAddr);
    }
}

public class STX_ZeropageY : STX
{
    public STX_ZeropageY()
    {
        SubTasks = ZeropageYAddressing();
        SubTasks.Add(WriteXToMemoryAtAddr);
    }
}

public class STX_Absolute : STX
{
    public STX_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add(WriteXToMemoryAtAddr);
    }
}
