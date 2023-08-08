using Emu6502;

var memory = new byte[0xffff];
memory[0xfffc] = Cpu.Instructions.Test;
memory[0xfffd] = 0x42;

Emu6502.Cpu cpu = new(memory);
cpu.Reset();
cpu.Execute(3);

Console.WriteLine("Done");
