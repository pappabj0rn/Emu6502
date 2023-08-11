﻿namespace Emu6502.Instructions;

public abstract class ADC : Instruction
{
    protected void AddMemoryAndCarryToAccumulator(
        ICpu cpu,
        ushort? addr = null)
    {
        var op1 = cpu.FetchMemory(addr);
        var result = (ushort)(op1
            + cpu.Registers.A
            + (cpu.Flags.C ? 1 : 0));
        
        var op1Positive = (op1 & 0x80) == 0x00;
        var aPositive = (cpu.Registers.A & 0x80) == 0x00;
        
        cpu.Registers.A = (byte)(result & 0xff);
        cpu.Flags.N = (cpu.Registers.A & 0x80) > 0;
        cpu.Flags.V = ((op1Positive && aPositive) || (!op1Positive && !aPositive)) 
                      && cpu.Flags.N == aPositive;
        cpu.Flags.Z = cpu.Registers.A == 0;
        cpu.Flags.C = result > 0xff;
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