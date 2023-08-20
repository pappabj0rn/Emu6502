namespace Emu6502.Instructions;

public abstract class ROL : Instruction
{
    protected ushort Value;

    protected void ReadMemoryToValue(ICpu cpu)
    {
        Value = cpu.FetchMemory(Addr);
    }

    protected void ShiftValueLeft(ICpu cpu)
    {
        Value = (ushort)((Value << 1)
            | (cpu.Flags.C
                ? 0x01
                : 0x00));
        cpu.State.Tick();
    }

    protected void WriteValueToAddr(ICpu cpu)
    {
        cpu.WriteMemory((byte)Value, Addr);
        cpu.Flags.C = Value > 0xFF;
        cpu.UpdateNZ((byte)Value);
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

public class ROL_Accumulator : ROL
{
    public ROL_Accumulator()
    {
        SubTasks = new() {
            (cpu) => {
                Value = (ushort)((cpu.Registers.A << 1)
                    | (cpu.Flags.C
                        ? 0x01
                        : 0x00));
                cpu.SetRegister(Register.A, (byte)Value);
                cpu.Flags.C = Value > 0xFF;
                cpu.State.Tick();
            }
        };
    }
}

public class ROL_Zeropage : ROL
{
    public ROL_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.AddRange(AslMemoryInstructions());
    }
}

public class ROL_ZeropageX : ROL
{
    public ROL_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.AddRange(AslMemoryInstructions());
    }
}

public class ROL_Absolute : ROL
{
    public ROL_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.AddRange(AslMemoryInstructions());
    }
}

public class ROL_AbsoluteX : ROL
{
    public ROL_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing(addCyclePenalty: true);
        SubTasks.AddRange(AslMemoryInstructions());
    }
}
