using Emu6502;
using System.Diagnostics;
using Instr = Emu6502.Cpu.Instructions;

byte[] memory = File.ReadAllBytes("6502_functional_test.bin");

Cpu cpu = new(memory);
cpu.Reset();
cpu.Registers.PC = 0x0400;

var trapMonitor = new ushort[6];
ushort lastPC = 0xffff;
trapMonitor[0] = 0xff;
var tIndex = 0;

//for(int i = 0; i < 1000; i++)
var cycles = 0;
var sw = new Stopwatch();
sw.Start();
while (true)
{


//    Console.SetCursorPosition(0, 0);
////    if (cycles % 10_000 == 0)
//    Console.WriteLine(
//        $"A: 0x{cpu.Registers.A:X2}   " +
//        $"X: 0x{cpu.Registers.X:X2}   " +
//        $"Y: 0x{cpu.Registers.Y:X2}   " +
//        $"SR: {ToNamedSr(cpu.Flags.GetSR())}   " +
//        $"SP: 0x{cpu.Registers.SP:X2}   " +
//        $"PC: 0x{cpu.Registers.PC:X4}   " +
//        $"RW: {(cpu.Pins.RW ? 'R' : 'W')}   " +
//        $"Addr: 0x{cpu.Pins.GetAddr():X4}   " +
//        $"Data: 0x{cpu.Pins.GetData():X2}   " +
//        $"Instr: {cpu.State.Instruction?.GetType().Name}");

    //Console.SetCursorPosition(0, 5);


    if (lastPC != cpu.Registers.PC)
    {
        //Console.WriteLine(
        //    $"A: 0x{cpu.Registers.A:X2}   " +
        //    $"X: 0x{cpu.Registers.X:X2}   " +
        //    $"Y: 0x{cpu.Registers.Y:X2}   " +
        //    $"SR: {ToNamedSr(cpu.Flags.GetSR())}   " +
        //    $"SP: 0x{cpu.Registers.SP:X2}   " +
        //    $"PC: 0x{cpu.Registers.PC:X4}   " +
        //    $"RW: {(cpu.Pins.RW ? 'R' : 'W')}   " +
        //    $"Addr: 0x{cpu.Pins.GetAddr():X4}   " +
        //    $"Data: 0x{cpu.Pins.GetData():X2}   " +
        //    $"Instr: {cpu.State.Instruction?.GetType().Name}");

        lastPC = cpu.Registers.PC;
        trapMonitor[tIndex++] = cpu.Registers.PC;
        tIndex %= 6;
    }


    cpu.Execute(1);
    cycles++;

    if (trapMonitor[0] == trapMonitor[3]
        && trapMonitor[1] == trapMonitor[4]
        && trapMonitor[2] == trapMonitor[5])
    {
        sw.Stop();
        Console.WriteLine($"Trapped at 0x{trapMonitor.First():X4}, sw: {sw.Elapsed}");
        if (trapMonitor[0]==0x3469)
            Console.WriteLine("Success, if you get here everything went well");
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

    //await Task.Delay(1);
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

Console.WriteLine($"Done in {cycles} cycles");
