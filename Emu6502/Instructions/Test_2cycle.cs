namespace Emu6502.Instructions;

public class Test_2cycle : Instruction
{
    public override void Execute(ICpu cpu)
    {
        if (cpu.State.InstructionSubstate == 0)
        {
            Console.WriteLine("test instruction cycle 1");
            cpu.Registers.X = 1;
            cpu.State.Tick();
            cpu.State.InstructionSubstate++;
        }

        if (cpu.State.Halted) return;
        Console.WriteLine("test instruction cycle 2");
        cpu.Registers.X = 2;
        cpu.State.Tick();

        cpu.State.ClearInstruction();
    }
}
