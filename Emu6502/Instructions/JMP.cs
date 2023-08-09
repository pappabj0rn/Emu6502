namespace Emu6502.Instructions;

public class JMP_Absolute : Instruction
{
    public override void Execute(ICpu cpu)
    {
        var low = cpu.FetchMemory();
        var high = cpu.FetchMemory();

        cpu.Registers.PC = (ushort)(low + (high << 8));
        cpu.State.ClearInstruction();
        
    }
}