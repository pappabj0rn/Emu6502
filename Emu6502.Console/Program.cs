using Emu6502;

var memory = new byte[0xffff];

memory[0x0000] = Cpu.Instructions.JMP_Absolute;
memory[0x0001] = 0x10;
memory[0x0002] = 0x00;
memory[0x0010] = Cpu.Instructions.LDA_Immediate;
memory[0x0011] = 0x42;

Cpu cpu = new(memory);
cpu.Reset();
cpu.Execute(5);
Console.WriteLine($"Value of A: 0x{cpu.Registers.A:X}");

Console.WriteLine("Done");
