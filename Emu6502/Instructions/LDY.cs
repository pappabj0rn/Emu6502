namespace Emu6502.Instructions;

public abstract class LDY : Instruction
{
    protected void LoadMemoryIntoY(
        ICpu cpu,
        ushort? addr = null)
    {
        cpu.SetRegister(Register.Y, cpu.FetchMemory(addr));
    }
}

public class LDY_Immediate : LDY
{
    public LDY_Immediate()
    {
        SubTasks = new()
        {
            (cpu) => LoadMemoryIntoY(cpu)
        };
    }
}

public class LDY_Zeropage : LDY
{
    public LDY_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoY(cpu, Addr));
    }
}

public class LDY_ZeropageX : LDY
{
    public LDY_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoY(cpu, Addr));
    }
}

public class LDY_Absolute : LDY
{
    public LDY_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoY(cpu, Addr));
    }
}

public class LDY_AbsoluteX : LDY
{
    public LDY_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => LoadMemoryIntoY(cpu, Addr));
    }
}
