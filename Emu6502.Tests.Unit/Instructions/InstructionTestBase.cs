namespace Emu6502.Tests.Unit.Instructions;

public abstract class InstructionTestBase
{
    protected Cpu Cpu;
    protected byte[] Memory;

    public InstructionTestBase()
    {
        Memory = new byte[0xffff];
        Cpu = new Cpu(Memory);
        Cpu.Reset();
    }
}