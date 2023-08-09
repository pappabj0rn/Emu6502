namespace Emu6502.Instructions;

public abstract class Instruction
{
    public abstract void Execute(ICpu cpu);
}

public class InvalidOperation : Instruction
{
    public override void Execute(ICpu cpu)
    {
        throw new InvalidOperationException();
    }
}
