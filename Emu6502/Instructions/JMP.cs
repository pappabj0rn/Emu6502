namespace Emu6502.Instructions;

public class JMP_Absolute : Instruction
{
    private ushort _addr = 0;

    public JMP_Absolute()
    {
        SubTasks = new() {
            (cpu) => _addr = cpu.FetchMemory(),
            (cpu) => {
                _addr += (ushort)(cpu.FetchMemory() << 8);
                cpu.Registers.PC = _addr;
            }
        };
    }
}