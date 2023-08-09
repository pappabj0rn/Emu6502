using Emu6502;

var memory = new byte[0xffff];
memory[0x00] = Cpu.Instructions.Test_2cycle;
memory[0x01] = 0x42;

Cpu cpu = new(memory);
cpu.Reset();
cpu.Execute(3);

Console.WriteLine("Done");
