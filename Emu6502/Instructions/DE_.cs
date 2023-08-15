namespace Emu6502.Instructions;

public abstract class DE_ : Instruction
{
    byte value;

    protected List<Action<ICpu>> DecrementMemoryByOne()
    {
        return new()
        {
            (cpu) => { value = cpu.FetchMemory(Addr); },
            (cpu) => { 
                value--;
                cpu.State.Tick();
            },
            (cpu) => { cpu.WriteMemory(value, Addr); }
        };
    }
}

public class DEC_Zeropage : DE_
{
    public DEC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.AddRange(DecrementMemoryByOne());
    }
}

public class DEC_ZeropageX : DE_
{
    public DEC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.AddRange(DecrementMemoryByOne());
    }
}

public class DEC_Absolute : DE_
{
    public DEC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.AddRange(DecrementMemoryByOne());
    }
}

public class DEC_AbsoluteX : DE_
{
    public DEC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.AddRange(DecrementMemoryByOne());
    }
}

public class DEX : DE_
{
    public DEX()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.Registers.X--;
                cpu.State.Tick();
            }
        };
    }

}

public class DEY : DE_
{
    public DEY()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.Registers.Y--;
                cpu.State.Tick();
            }
        };
    }

}
