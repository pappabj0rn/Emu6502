﻿namespace Emu6502.Instructions;

public abstract class LSR : Instruction
{
    protected ushort Value;

    protected void ReadMemoryToValue(ICpu cpu)
    {
        Value = cpu.FetchMemory(Addr);
    }

    protected void ShiftValueRight(ICpu cpu)
    {
        cpu.Flags.C = (Value & 0x01) == 1;
        Value = (ushort)(Value >> 1);
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

public class LSR_Accumulator : LSR
{
    public LSR_Accumulator()
    {
        SubTasks = new() {
            (cpu) => {
                Value = (ushort)(cpu.Registers.A >> 1);
                cpu.Flags.C = (cpu.Registers.A & 0x01) == 1;
                cpu.SetRegister(Register.A, (byte)Value);                
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
