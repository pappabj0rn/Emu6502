﻿namespace Emu6502.Instructions;

public abstract class ADC : Instruction
{
    protected virtual byte FetchOp2(ICpu cpu, ushort? addr = null)
    {
        return cpu.FetchMemory(addr);
    }

    protected void AddMemoryAndCarryToAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var op1 = cpu.Registers.A;
        var op2 = FetchOp2(cpu, addr);

        byte max = 0xff;
        if (cpu.Flags.D)
        {
            op1 = DecodeDecimal(op1);
            op2 = DecodeDecimal(op2);
            max = 0x63;
        }

        var result = (ushort)(op1
            + op2
            + (cpu.Flags.C ? 1 : 0));

        var op1Positive = (op1 & 0x80) == 0x00;
        var op2Positive = (op2 & 0x80) == 0x00;

        cpu.SetRegister(Register.A, cpu.Flags.D ? EncodeDecimal(result) : (byte)result);
        cpu.Flags.C = result > max;
        cpu.Flags.V = ((op1Positive && op2Positive) || (!op1Positive && !op2Positive))
                      && cpu.Flags.N == op2Positive
                      && !cpu.Flags.D;
    }

    private byte EncodeDecimal(int value)
    {
        int digitOne = (value / 10);
        int digitTwo = (value - digitOne * 10);

        return (byte)((digitOne % 10) << 4 | digitTwo);
    }

    private byte DecodeDecimal(byte value)
    {
        return (byte)((((value&  0xF0) >> 4) * 10) + (value & 0x0F));
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
    public ADC_Absolute()
    {
        SubTasks = AbsoluteAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_AbsoluteX : ADC
{
    public ADC_AbsoluteX()
    {
        SubTasks = AbsoluteXAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_AbsoluteY : ADC
{
    public ADC_AbsoluteY()
    {
        SubTasks = AbsoluteYAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_Zeropage : ADC
{
    public ADC_Zeropage()
    {
        SubTasks = ZeropageAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_ZeropageX : ADC
{
    public ADC_ZeropageX()
    {
        SubTasks = ZeropageXAddressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_IndirectX : ADC
{
    public ADC_IndirectX()
    {
        SubTasks = IndirectXAdressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}

public class ADC_IndirectY : ADC
{
    public ADC_IndirectY()
    {
        SubTasks = IndirectYAdressing();
        SubTasks.Add((cpu) => AddMemoryAndCarryToAccumulator(cpu, Addr));
    }
}