namespace Emu6502.Instructions;

public abstract class SE_ : Instruction
{
    protected abstract Action<ICpu> SetBit { get; }

    public SE_()
    {
        SubTasks = new()
        {
            (cpu) => {
                SetBit(cpu);
                cpu.State.Tick();
            }
        };
    }
}

public class SEC : SE_
{
    protected override Action<ICpu> SetBit => (cpu) => cpu.Flags.C = true;
}

public class SED : SE_
{
    protected override Action<ICpu> SetBit => (cpu) => cpu.Flags.D = true;
}

public class SEI : SE_
{
    protected override Action<ICpu> SetBit => (cpu) => cpu.Flags.I = true;
}
