namespace Emu6502.Instructions;

public abstract class LDA : Instruction
{
    protected void LoadAccumulatorWithMemory(
        ICpu cpu, 
        ushort? addr = null)
    {
        cpu.Registers.A = cpu.FetchMemory(addr);
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;
    }
}

public class LDA_Immediate : LDA
{
    public LDA_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => LoadAccumulatorWithMemory(cpu)
        };
    }
}

public class LDA_Absolute : LDA
{
    public LDA_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}

public class LDA_AbsoluteX : LDA
{
    public LDA_AbsoluteX()
    {
        SubTasks = new() {
            (cpu) => Addr = (ushort)(cpu.FetchMemory() + cpu.Registers.X),
            (cpu) => {
                if(Addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => Addr += (ushort)(cpu.FetchMemory() << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, Addr)
        };
    }
}

public class LDA_AbsoluteY : LDA
{
    public LDA_AbsoluteY()
    {
        SubTasks = new() {
            (cpu) => Addr = (ushort)(cpu.FetchMemory() + cpu.Registers.Y),
            (cpu) => {
                if(Addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => Addr += (ushort)(cpu.FetchMemory() << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, Addr)
        };
    }
}

public class LDA_Zeropage : LDA
{
    public LDA_Zeropage()
    {
        SubTasks = new() {
            (cpu) => Addr = cpu.FetchMemory(),
            (cpu) => LoadAccumulatorWithMemory(cpu, Addr)
        };
    }
}

public class LDA_ZeropageX : LDA
{
    public LDA_ZeropageX()
    {
        SubTasks = new() {
            (cpu) => Addr = cpu.FetchMemory(),
            (cpu) => {
                Addr += cpu.Registers.X;
                cpu.State.Tick();
            },
            (cpu) => LoadAccumulatorWithMemory(cpu, (ushort)(Addr & 0x00ff))
        };
    }
}

public class LDA_PreIndexedIndirectZeropageX : LDA
{
    public LDA_PreIndexedIndirectZeropageX()
    {
        SubTasks = new() {
            (cpu) => IndAddr = cpu.FetchMemory(),
            (cpu) => {
                IndAddr += cpu.Registers.X;
                cpu.State.Tick();
            },
            (cpu) => Addr = cpu.FetchMemory((ushort)(IndAddr & 0x00ff)),
            (cpu) => Addr += (ushort)(cpu.FetchMemory((ushort)((IndAddr + 1) & 0x00ff)) << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, Addr)
        };
    }
}

public class LDA_PostIndexedIndirectZeropageY : LDA
{
    public LDA_PostIndexedIndirectZeropageY()
    {
        SubTasks = new() {
            (cpu) => IndAddr = cpu.FetchMemory(),
            (cpu) => Addr = (ushort)(cpu.FetchMemory(IndAddr) + cpu.Registers.Y),
            (cpu) => {
                if(Addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => Addr += (ushort)(cpu.FetchMemory((ushort?)(IndAddr + 1)) << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, Addr)
        };
    }
}
