namespace Emu6502.Instructions;

public class LDA_Immediate : Instruction
{
    public override void Execute(ICpu cpu)
    {
        cpu.Registers.A = cpu.FetchMemory();
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;

        cpu.State.ClearInstruction();
    }
}

public class LDA_Absolute : Instruction
{
    public override void Execute(ICpu cpu)
    {
        ushort addr = 0;
        if (cpu.State.InstructionSubstate == 0)
        {
            addr = cpu.FetchMemory();
            cpu.State.InstructionSubstate++;
            if (cpu.State.Halted) return;
        }

        if (cpu.State.InstructionSubstate == 1)
        {
            addr += (ushort)(cpu.FetchMemory() << 8);
            cpu.State.InstructionSubstate++;
            if (cpu.State.Halted) return;
        }

        cpu.Registers.A = cpu.FetchMemory(addr);
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;

        cpu.State.ClearInstruction();
    }
}
