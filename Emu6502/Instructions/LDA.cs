namespace Emu6502.Instructions;

public abstract class LDA : Instruction
{
    protected void LoadAccumulatorWithMemory(
        ICpu cpu, 
        ushort? addr = null)
    {
        cpu.SetRegister(Register.A, cpu.FetchMemory(addr));
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
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}

public class LDA_AbsoluteY : LDA
{
    public LDA_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}

public class LDA_Zeropage : LDA
{
    public LDA_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}

public class LDA_ZeropageX : LDA
{
    public LDA_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}

public class LDA_IndirectX : LDA
{
    public LDA_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}

public class LDA_IndirectY : LDA
{
    public LDA_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => LoadAccumulatorWithMemory(cpu, Addr));
    }
}
