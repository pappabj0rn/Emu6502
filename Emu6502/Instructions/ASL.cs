namespace Emu6502.Instructions;

public abstract class ASL : Instruction
{
    protected ushort _value;

    protected void ReadMemoryToValue(ICpu cpu)
    {
        _value = cpu.FetchMemory(Addr);
    }

    protected void ShiftValueLeft(ICpu cpu)
    {
        _value = (ushort)(_value << 1);
        cpu.State.Tick();
    }

    protected void WriteValueToAddr(ICpu cpu)
    {
        cpu.WriteMemory((byte)_value, Addr);
        cpu.Flags.C = _value > 0xFF;
        cpu.UpdateNZ((byte)_value);
    }

    protected List<Action<ICpu>> AslMemoryInstructions()
    {
        return new()
        {
            ReadMemoryToValue,
            ShiftValueLeft,
            WriteValueToAddr
        };
    }
}

public class ASL_Accumulator : ASL
{
    public ASL_Accumulator()
    {
        SubTasks = new() {
            (cpu) => {
                _value = (ushort)(cpu.Registers.A << 1);
                cpu.SetRegister(Register.A, (byte)_value);
                cpu.Flags.C = _value > 0xFF;
                cpu.State.Tick();
            }
        };
    }
}

public class ASL_Zeropage : ASL
{
    public ASL_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.AddRange(AslMemoryInstructions());
    }
}

public class ASL_ZeropageX : ASL
{
    public ASL_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.AddRange(AslMemoryInstructions());
    }
}

public class ASL_Absolute : ASL
{
    public ASL_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.AddRange(AslMemoryInstructions());
    }
}

public class ASL_AbsoluteX : ASL
{
    public ASL_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.AddRange(AslMemoryInstructions());
    }
}
