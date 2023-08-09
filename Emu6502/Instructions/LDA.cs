namespace Emu6502.Instructions;

/*
Load Accumulator with Memory

addressing	    assembler	    opc	bytes	cycles°
absolute,X	    LDA oper,X	    BD	3	    4* 
absolute,Y	    LDA oper,Y	    B9	3	    4* 
(indirect,X)	LDA (oper,X)	A1	2	    6  
(indirect),Y	LDA (oper),Y	B1	2	    5* 

°inc fetching instruction
 */

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
    private ushort _addr = 0;

    public LDA_Absolute()
    {
        SubTasks = new() { 
            (cpu) => _addr = cpu.FetchMemory(),
            (cpu) => _addr += (ushort)(cpu.FetchMemory() << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, _addr)
        };
    }
}

public class LDA_Zeropage : LDA
{
    private ushort _addr = 0;

    public LDA_Zeropage()
    {
        SubTasks = new() {
            (cpu) => _addr = cpu.FetchMemory(),
            (cpu) => LoadAccumulatorWithMemory(cpu, _addr)
        };
    }
}

public class LDA_ZeropageX : LDA
{
    private ushort _addr = 0;

    public LDA_ZeropageX()
    {
        SubTasks = new() {
            (cpu) => _addr = cpu.FetchMemory(),
            (cpu) => _addr += cpu.FetchX(),
            (cpu) => LoadAccumulatorWithMemory(cpu, (ushort)(_addr & 0x00ff))
        };
    }
}
