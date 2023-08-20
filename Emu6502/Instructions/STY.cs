namespace Emu6502.Instructions;

public class STY : Instruction
{
    protected void WriteYToMemoryAtAddr(ICpu cpu)
    {
        cpu.WriteMemory(cpu.Registers.Y, Addr);
    }
}

public class STY_Zeropage : STY
{
    public STY_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add(WriteYToMemoryAtAddr);
    }
}

public class STY_ZeropageX : STY
{
    public STY_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add(WriteYToMemoryAtAddr);
    }
}

public class STY_Absolute : STY
{
    public STY_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add(WriteYToMemoryAtAddr);
    }
}