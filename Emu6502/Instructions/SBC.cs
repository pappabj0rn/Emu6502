namespace Emu6502.Instructions;

/*
 Subtract Memory from Accumulator with Borrow

A - M - C -> A              N	Z	C	I	D	V
                            +	+	+	-	-	+
addressing	    assembler	    opc	bytes	cycles²
---------------------------------------------------
immediate	    SBC #oper	    E9	2	    2  
zeropage	    SBC oper	    E5	2	    3  
zeropage,X	    SBC oper,X	    F5	2	    4  
absolute	    SBC oper	    ED	3	    4  
absolute,X	    SBC oper,X	    FD	3	    4* 
absolute,Y	    SBC oper,Y	    F9	3	    4* 
(indirect,X)	SBC (oper,X)	E1	2	    6
(indirect),Y	SBC (oper),Y	F1	2	    5* 

 */
public abstract class SBC : Instruction
{
    protected void SubtractMemoryWithBorrowFromAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        //TODO
        return;
        var result = (ushort)(cpu.FetchMemory(addr)
            + cpu.Registers.A
            + (cpu.Flags.C ? 1 : 0));

        cpu.Registers.A = (byte)(result & 0xff);
        cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
        cpu.Flags.Z = cpu.Registers.A == 0;
        cpu.Flags.C = result > 0xff;
    }
}

public class SBC_Immediate : SBC
{
    public SBC_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu)
        };
    }
}

public class SBC_Absolute : SBC
{
    public SBC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_AbsoluteX : SBC
{
    public SBC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_AbsoluteY : SBC
{
    public SBC_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_Zeropage : SBC
{
    public SBC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_ZeropageX : SBC
{
    public SBC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_IndirectX : SBC
{
    public SBC_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}

public class SBC_IndirectY : SBC
{
    public SBC_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => SubtractMemoryWithBorrowFromAccumulator(cpu, Addr));
    }
}
