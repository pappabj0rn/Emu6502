namespace Emu6502.Instructions;

public class LDA_Immediate : Instruction
{
    public override void Execute(Cpu cpu)
    {
        cpu.Registers.A = cpu.FetchMemory();
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;

        cpu.State.ClearInstruction();
    }
}
