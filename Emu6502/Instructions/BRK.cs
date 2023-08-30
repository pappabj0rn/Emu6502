namespace Emu6502.Instructions;

public class BRK : Instruction
{
    public BRK()
    {
        SubTasks = new()
        {
            (cpu) => {                
                //throw away
                cpu.FetchMemory();
            },
            (cpu) => {
                cpu.WriteMemory((byte)(cpu.Registers.PC >> 8), (ushort?)(0x0100 | cpu.Registers.SP--));
            },
            (cpu) => {
                cpu.WriteMemory((byte)cpu.Registers.PC, (ushort?)(0x0100 | cpu.Registers.SP--));
            },
            (cpu) => {
                cpu.WriteMemory(cpu.Flags.GetSR(), (ushort?)(0x0100 | cpu.Registers.SP--));
            },
            (cpu) => {
                cpu.Registers.PC = cpu.FetchMemory(0xFFFE);
            },
            (cpu) => {
                cpu.Flags.I = true;
                cpu.Registers.PC |= (ushort)(cpu.FetchMemory(0xFFFF) << 8);
            }
        };
    }
}
