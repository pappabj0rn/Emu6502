namespace Emu6502.Instructions;

public class STA : Instruction
{
    protected void WriteAToMemoryAtAddr(ICpu cpu) 
    { 
        cpu.WriteMemory(cpu.Registers.A, Addr); 
    }
}

public class STA_Zeropage : STA
{
    public STA_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add(WriteAToMemoryAtAddr);
    }
}

public class STA_ZeropageX : STA
{
    public STA_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add(WriteAToMemoryAtAddr);
    }
}

public class STA_Absolute : STA
{
    public STA_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add(WriteAToMemoryAtAddr);
    }
}

public class STA_AbsoluteX : STA
{
    public STA_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.Add(WriteAToMemoryAtAddr);
    }
}

public class STA_AbsoluteY : STA
{
    public STA_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing(addCyclePenalty: true);
        SubTasks.Add(WriteAToMemoryAtAddr);
    }
}

public class STA_IndirectX : STA
{
    public STA_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add(WriteAToMemoryAtAddr);
    }
}

public class STA_IndirectY : STA
{
    public STA_IndirectY()
    {
        {
            SubTasks = IndirectYAdressing(addCyclePenalty: true);
            SubTasks.Add(WriteAToMemoryAtAddr);
        }
    }
}