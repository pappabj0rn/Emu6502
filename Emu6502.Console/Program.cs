using Emu6502;

var memory = new byte[0xffff];

memory[0x0000] = Cpu.Instructions.LDA_Zeropage;
memory[0x0001] = 0x0F;
memory[0x0002] = Cpu.Instructions.LDA_Immediate;
memory[0x0003] = 0x42;
memory[0x0004] = Cpu.Instructions.LDA_Immediate;
memory[0x0005] = 0x43;
//
memory[0x000F] = 0x01;

Cpu cpu = new(memory);
cpu.Reset();

for(int i = 0; i < 7; i++)
{
    cpu.Execute(1);
    Console.WriteLine($"Value of A: 0x{cpu.Registers.A:X}");
}


Console.WriteLine("Done");
