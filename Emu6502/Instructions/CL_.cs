namespace Emu6502.Instructions;

public abstract class CL_ : Instruction
{
    protected abstract Action<ICpu> ClearBit { get; }

    public CL_()
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

public class CLC : CL_
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.C = false;
}

public class CLD : CL_
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.D = false;
}

public class CLI : CL_
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.I = false;
}

public class CLV : CL_
{
    protected override Action<ICpu> ClearBit => (cpu) => cpu.Flags.V = false;
}
