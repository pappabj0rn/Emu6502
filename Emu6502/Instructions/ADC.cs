﻿namespace Emu6502.Instructions;

/*

Add Memory to Accumulator with Carry

A + M + C -> A, C
                            N	Z	C	I	D	V
                            +	+	+	-	-	+
addressing	    assembler	    opc	bytes	cycles²
immediate	    ADC #oper	    69	2	    2  
zeropage	    ADC oper	    65	2	    3  
zeropage,X	    ADC oper,X	    75	2	    4  
absolute	    ADC oper	    6D	3	    4  
absolute,X	    ADC oper,X	    7D	3	    4* 
absolute,Y	    ADC oper,Y	    79	3	    4* 
(indirect,X)	ADC (oper,X)	61	2	    6  
(indirect),Y	ADC (oper),Y	71	2	    5* 

 */

public abstract class ADC : Instruction
{
    protected void AddMemoryAndCarryToAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var result = (ushort)(cpu.FetchMemory(addr)
            + cpu.Registers.A
            + (cpu.Flags.C ? 1 : 0));

        cpu.Registers.A = (byte)(result & 0xff);
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;
        cpu.Flags.C = result > 0xff;        
    }
}

public class ADC_Immediate : ADC
{
    public ADC_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => AddMemoryAndCarryToAccumulator(cpu)
        };
    }
}

public class ADC_Absolute : ADC
{
    private ushort _addr;

    public ADC_Absolute()
    {
        SubTasks = new()
        {
            (cpu) => { _addr = cpu.FetchMemory(); },
            (cpu) => { _addr += (ushort)(cpu.FetchMemory() << 8); },
            (cpu) => AddMemoryAndCarryToAccumulator(cpu, _addr)
        };
    }
}
