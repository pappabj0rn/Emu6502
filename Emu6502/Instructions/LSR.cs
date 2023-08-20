namespace Emu6502.Instructions;

public abstract class LSR : Instruction
{
    protected ushort _value;

    protected void ReadMemoryToValue(ICpu cpu)
    {
        _value = cpu.FetchMemory(Addr);
    }

    protected void ShiftValueRight(ICpu cpu)
    {
        cpu.Flags.C = (_value & 0x01) == 1;
        _value = (ushort)(_value >> 1);
        cpu.State.Tick();
    }

    protected void WriteValueToAddr(ICpu cpu)
    {
        cpu.WriteMemory((byte)_value, Addr);
        cpu.UpdateNZ((byte)_value);
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

public class LSR_Accumulator : LSR
{
    public LSR_Accumulator()
    {
        SubTasks = new() {
            (cpu) => {
                _value = (ushort)(cpu.Registers.A >> 1);
                cpu.Flags.C = (cpu.Registers.A & 0x01) == 1;
                cpu.SetRegister(Register.A, (byte)_value);                
                cpu.State.Tick();
            }
        };
    }
}

public class LSR_Zeropage : LSR
{
    public LSR_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}

public class LSR_ZeropageX : LSR
{
    public LSR_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}

public class LSR_Absolute : LSR
{
    public LSR_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}

public class LSR_AbsoluteX : LSR
{
    public LSR_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.AddRange(LsrMemoryInstructions());
    }
}
