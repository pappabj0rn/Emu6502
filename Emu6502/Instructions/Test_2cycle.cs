namespace Emu6502.Instructions;

public class Test_2cycle : Instruction
{
    public Test_2cycle()
    {
        SubTasks = new() {
            (cpu) => {
                Console.WriteLine("test instruction cycle 1");
                cpu.Registers.X = 1;
                cpu.State.Tick();
            },
            (cpu) => {
                Console.WriteLine("test instruction cycle 2");
                cpu.Registers.X = 2;
                cpu.State.Tick();
            }
        };
    }
}
