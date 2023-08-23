namespace Emu6502.Instructions;

public class RTI : Instruction
{
    public RTI()
    {
        SubTasks = new()
        {
            (cpu) => {                
                //throw away
                cpu.FetchMemory();
            },
            (cpu) => {
                cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP++));
            },
            (cpu) => {
                cpu.Flags.SetSR(cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP++)));
            },
            (cpu) => {
                cpu.Registers.PC = cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP++));
            },
            (cpu) => {
                cpu.Registers.PC |= (ushort)(cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP)) << 8);
            }
        };
    }
}