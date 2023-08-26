using Emu6502;

var memory = new byte[0xffff];
ushort mem = 0;
memory[mem++] = Cpu.Instructions.LDA_Immediate;
memory[mem++] = 0x01;
memory[mem++] = Cpu.Instructions.CLC;
memory[mem++] = Cpu.Instructions.ADC_Immediate;
memory[mem++] = 0x01;
memory[mem++] = Cpu.Instructions.STA_Absolute;
memory[mem++] = 0x00;
memory[mem++] = 0x02;
memory[mem++] = Cpu.Instructions.JMP_Absolute;
memory[mem++] = 0x02;
memory[mem++] = 0x00;

Cpu cpu = new(memory);
cpu.Reset();

for(int i = 0; i < 1000; i++)
{
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

    cpu.Execute(1);
    switch (memory[0x0200])
    {
        case > 0x40 and < 0x5B:
            var pos = Console.GetCursorPosition();
            Console.SetCursorPosition(2, 2);
            Console.Write((char)memory[0x0200]);
            Console.SetCursorPosition(pos.Left, pos.Top);
            break;
    };
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
