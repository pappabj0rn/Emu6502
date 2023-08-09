namespace Emu6502.Instructions;

public class InvalidOperation : Instruction
{
    public override void Execute(ICpu cpu)
    {
        throw new InvalidOperationException();
    }
}
