namespace Emu6502.Instructions;

/*
LDA, Load Accumulator with Memory

M -> A
                                N	Z	C	I	D	V
                                +	+	-	-	-	-

addressing	    assembler	    opc	bytes	cycles°
-----------------------------------------------------
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

public class LDA_AbsoluteX : LDA
{
    private ushort _addr;

    public LDA_AbsoluteX()
    {
        SubTasks = new() {
            (cpu) => _addr = (ushort)(cpu.FetchMemory() + cpu.Registers.X),
            (cpu) => {
                if(_addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => _addr += (ushort)(cpu.FetchMemory() << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, _addr)
        };
    }
}

public class LDA_AbsoluteY : LDA
{
    private ushort _addr;

    public LDA_AbsoluteY()
    {
        SubTasks = new() {
            (cpu) => _addr = (ushort)(cpu.FetchMemory() + cpu.Registers.Y),
            (cpu) => {
                if(_addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
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

public class LDA_PreIndexedIndirectZeropageX : LDA
{
    private ushort _indAddr = 0;
    private ushort _addr = 0;

    public LDA_PreIndexedIndirectZeropageX()
    {
        SubTasks = new() {
            (cpu) => _indAddr = cpu.FetchMemory(),
            (cpu) => _indAddr += cpu.FetchX(),
            (cpu) => _addr = cpu.FetchMemory((ushort)(_indAddr & 0x00ff)),
            (cpu) => _addr += (ushort)(cpu.FetchMemory((ushort)((_indAddr + 1) & 0x00ff)) << 8),
            (cpu) => LoadAccumulatorWithMemory(cpu, _addr)
        };
    }
}
