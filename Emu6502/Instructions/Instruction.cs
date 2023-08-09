namespace Emu6502.Instructions;

public abstract class Instruction
{
    public abstract void Execute(Cpu cpu);
}

public class InvalidOperation : Instruction
{
    public override void Execute(Cpu cpu)
    {
        throw new NotImplementedException();
    }
}
