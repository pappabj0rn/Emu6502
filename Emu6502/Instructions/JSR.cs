namespace Emu6502.Instructions;

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.

public class JSR_Absolute : Instruction
{
    public JSR_Absolute()
    {
        SubTasks = new() {
            (cpu) => { Addr = cpu.FetchMemory(); },
            (cpu) => {                
                //internal operation
                cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP));
            },
            (cpu) => {
                cpu.WriteMemory((byte)(cpu.Registers.PC >> 8), (ushort?)(0x0100 | cpu.Registers.SP)); //todo move push to cpu
                cpu.Registers.SP--;
            },
            (cpu) => {
                cpu.WriteMemory((byte)cpu.Registers.PC, (ushort?)(0x0100 | cpu.Registers.SP));
                cpu.Registers.SP--;
            },
            (cpu) => { 
                Addr += (ushort)(cpu.FetchMemory() << 8);
                cpu.Registers.PC = (ushort)Addr!;
            }
        };
    }    
}
