namespace Emu6502.Instructions;

public class JMP_Absolute : Instruction
{
    public JMP_Absolute()
    {
        SubTasks = new() {
            (cpu) => Addr = cpu.FetchMemory(),
            (cpu) => {
                Addr += (ushort)(cpu.FetchMemory() << 8);
                cpu.Registers.PC = (ushort)Addr!;
            }
        };
    }
}

public class JMP_Indirect : Instruction
{
    public JMP_Indirect()
    {
        SubTasks = new() {
            (cpu) => IndAddr = cpu.FetchMemory(),
            (cpu) => { IndAddr += (ushort)(cpu.FetchMemory() << 8); },
            (cpu) => Addr = cpu.FetchMemory(IndAddr),
            (cpu) => {
                Addr += (ushort)(cpu.FetchMemory((ushort)(IndAddr + 1)) << 8);
                cpu.Registers.PC = (ushort)Addr!;
            }
        };
    }
}