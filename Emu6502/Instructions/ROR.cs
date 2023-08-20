namespace Emu6502.Instructions;

public abstract class ROR : Instruction
{
    protected ushort Value;

    protected void ReadMemoryToValue(ICpu cpu)
    {
        Value = cpu.FetchMemory(Addr);
    }

    protected void ShiftValueRight(ICpu cpu)
    {
        var carry = cpu.Flags.C
                ? 0x80
                : 0x00;
        cpu.Flags.C = (Value & 0x01) == 1;
        Value = (ushort)((Value >> 1) | carry);
        cpu.State.Tick();
    }

    protected void WriteValueToAddr(ICpu cpu)
    {
        cpu.WriteMemory((byte)Value, Addr);
        cpu.UpdateNZ((byte)Value);
    }

    protected List<Action<ICpu>> LsrMemoryInstructions()
    {
        return new()
        {
            ReadMemoryToValue,
            ShiftValueRight,
            WriteValueToAddr
        };
    }
}

public class ROR_Accumulator : ROR
{
    public ROR_Accumulator()
    {
        SubTasks = new() {
            (cpu) => {
                var carry = cpu.Flags.C
                        ? 0x80
                        : 0x00;
                Value = (ushort)((cpu.Registers.A >> 1) | carry);
                cpu.Flags.C = (cpu.Registers.A & 0x01) == 1;
                cpu.SetRegister(Register.A, (byte)Value);                
                cpu.State.Tick();
            }
        };
    }
}

public class ROR_Zeropage : ROR
{
    public ROR_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}

public class ROR_ZeropageX : ROR
{
    public ROR_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}

public class ROR_Absolute : ROR
{
    public ROR_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}

public class ROR_AbsoluteX : ROR
{
    public ROR_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}
