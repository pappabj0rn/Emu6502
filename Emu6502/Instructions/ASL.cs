﻿namespace Emu6502.Instructions;

public abstract class ASL : Instruction
{
    protected ushort Value;

    protected void ReadMemoryToValue(ICpu cpu)
    {
        Value = cpu.FetchMemory(Addr);
    }

    protected void ShiftValueLeft(ICpu cpu)
    {
        Value = (ushort)(Value << 1);
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

public class ASL_Accumulator : ASL
{
    public ASL_Accumulator()
    {
        SubTasks = new() {
            (cpu) => {
                Value = (ushort)(cpu.Registers.A << 1);
                cpu.SetRegister(Register.A, (byte)Value);
                cpu.Flags.C = Value > 0xFF;
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
