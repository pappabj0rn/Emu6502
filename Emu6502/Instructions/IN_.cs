namespace Emu6502.Instructions;

public abstract class IN_ : Instruction
{
    byte value;
    
    protected List<Action<ICpu>> IncrementMemoryByOne()
    {
        return new()
        {
            (cpu) => { value = cpu.FetchMemory(Addr); },
            (cpu) => {
                value++;
                cpu.State.Tick();
            },
            (cpu) => { 
                cpu.WriteMemory(value, Addr);
                cpu.UpdateNZ(value);
            }
        };
    }
}

public class INC_Zeropage : IN_
{
    public INC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.AddRange(IncrementMemoryByOne());
    }
}

public class INC_ZeropageX : IN_
{
    public INC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.AddRange(IncrementMemoryByOne());
    }
}

public class INC_Absolute : IN_
{
    public INC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.AddRange(IncrementMemoryByOne());
    }
}

public class INC_AbsoluteX : IN_
{
    public INC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.AddRange(IncrementMemoryByOne());
    }
}

public class INX : IN_
{
    public INX()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.X, (byte)(cpu.Registers.X + 1));                
                cpu.State.Tick();
            }
        };
    }

}

public class INY : IN_
{
    public INY()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.Y, (byte)(cpu.Registers.Y + 1));
                cpu.State.Tick();
            }
        };
    }

}