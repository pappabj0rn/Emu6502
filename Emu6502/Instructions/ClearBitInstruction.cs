namespace Emu6502.Instructions;

public abstract class ClearBitInstruction : Instruction
{
    protected abstract Action<ICpu> ClearBit {get;}

    public ClearBitInstruction()
    {
        SubTasks = new()
        {
            (cpu) => {
                ClearBit(cpu);
                cpu.State.Tick(); 
            }
        };
    }
}

public class CLC : ClearBitInstruction
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.C = false;
}

public class CLD : ClearBitInstruction
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.D = false;
}

public class CLI : ClearBitInstruction
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.I = false;
}

public class CLV : ClearBitInstruction
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.V = false;
}
