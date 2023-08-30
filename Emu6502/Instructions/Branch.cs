namespace Emu6502.Instructions;

public abstract class Branch : Instruction
{
    protected ushort NewPC;
    protected abstract bool DoNotBranch(ICpu cpu);

    public Branch()
    {
        SubTasks = new() {
            (cpu) =>
            {
                Addr = cpu.FetchMemory();

                if(DoNotBranch(cpu))
                {
                    cpu.State.InstructionSubstate+=2;
                }
            },
            (cpu) =>
            {
                NewPC = (Addr & 0x80) == 0x00
                    ? Forward(cpu)
                    : Reverse(cpu);

                if((cpu.Registers.PC & 0xFF00) != (NewPC & 0xFF00))
                {
                    cpu.State.Tick();
                }
            },
            (cpu) =>
            {
                cpu.State.Tick();
                cpu.Registers.PC = NewPC;
            }
        };
    }

    private ushort Reverse(ICpu cpu)
    {
        return (ushort)(cpu.Registers.PC - (byte)(0x100 - Addr!));
    }

    private ushort Forward(ICpu cpu)
    {
        return (ushort)(cpu.Registers.PC + (byte)Addr!);
    }
}

public class BCC : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => cpu.Flags.C;
}

public class BCS : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => !cpu.Flags.C;
}

public class BNE : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => cpu.Flags.Z;
}

public class BEQ : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => !cpu.Flags.Z;
}

public class BPL : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => cpu.Flags.N;
}

public class BMI : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => !cpu.Flags.N;
}

public class BVC : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => cpu.Flags.V;
}

public class BVS : Branch
{
    protected override bool DoNotBranch(ICpu cpu) => !cpu.Flags.V;
}
