using Emu6502;
using Instr = Emu6502.Cpu.Instructions;

var memory = new byte[0x10000];
byte[] fileBytes = File.ReadAllBytes("6502_functional_test.bin");
//ushort mem = 0;
//memory[mem++] = Instr.LDA_Immediate; //init
//memory[mem++] = 0x40;
//memory[mem++] = Instr.LDX_Immediate;
//memory[mem++] = 0x5B;
//memory[mem++] = Instr.STA_Absolute;
//memory[mem++] = 0x00;
//memory[mem++] = 0x02;
//
//memory[mem++] = Instr.INC_Absolute; //inc
//memory[mem++] = 0x00;
//memory[mem++] = 0x02;
//memory[mem++] = Instr.CPX_Absolute;
//memory[mem++] = 0x00;
//memory[mem++] = 0x02;
//memory[mem++] = Instr.BEQ;//reset if x reached
//memory[mem++] = 0x04;
//memory[mem++] = 0x00;
//
//memory[mem++] = Instr.JMP_Absolute;//cont inc
//memory[mem++] = 0x07;
//memory[mem++] = 0x00;

Cpu cpu = new(fileBytes);
cpu.Reset();
cpu.Registers.PC = 0x0400;

var trapMonitor = new ushort[6];
ushort lastPC = 0xffff;
trapMonitor[0] = 0xff;
var tIndex = 0;

//for(int i = 0; i < 1000; i++)
while (true)
{
    cpu.Execute(1);

    Console.SetCursorPosition(0, 0);
    Console.WriteLine(
        $"A: 0x{cpu.Registers.A:X2}   " +
        $"X: 0x{cpu.Registers.X:X2}   " +
        $"Y: 0x{cpu.Registers.Y:X2}   " +
        $"SR: {ToNamedSr(cpu.Flags.GetSR())}   " +
        $"SP: 0x{cpu.Registers.SP:X2}   " +
        $"PC: 0x{cpu.Registers.PC:X4}   " +
        $"RW: {(cpu.Pins.RW ? 'R' : 'W')}   " +
        $"Addr: 0x{cpu.Pins.GetAddr():X4}   " +
        $"Data: 0x{cpu.Pins.GetData():X2}");

    Console.SetCursorPosition(0, 5);

    
    if(lastPC != cpu.Registers.PC)
    {
        lastPC = cpu.Registers.PC;
        trapMonitor[tIndex++] = cpu.Registers.PC;
        tIndex %= 6;
    }
    if (trapMonitor[0] == trapMonitor[3]
        && trapMonitor[1] == trapMonitor[4]
        && trapMonitor[2] == trapMonitor[5])
    {
        Console.WriteLine($"Trapped at 0x{trapMonitor[1]:X4}");
        break;
    }
    //switch (memory[0x0200])
    //{
    //    case > 0x40 and < 0x5B:
    //        var pos = Console.GetCursorPosition();
    //        Console.SetCursorPosition(2, 2);
    //        Console.Write((char)memory[0x0200]);
    //        Console.SetCursorPosition(pos.Left, pos.Top);
    //        break;
    //};

    await Task.Delay(1);
}

string ToNamedSr(byte b)
{
    return ""
        + (((b & 0x80) > 0) ? 'N' : 'n')
        + (((b & 0x40) > 0) ? 'V' : 'v')
        + '-'
        + (((b & 0x10) > 0) ? 'B' : 'b')
        + (((b & 0x08) > 0) ? 'D' : 'd')
        + (((b & 0x04) > 0) ? 'I' : 'i')
        + (((b & 0x02) > 0) ? 'Z' : 'z')
        + (((b & 0x01) > 0) ? 'C' : 'c')
        ;
}

Console.WriteLine("Done");
