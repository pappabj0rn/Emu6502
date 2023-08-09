namespace Emu6502.Instructions;

public class JMP_Absolute : Instruction
{
    private ushort _addr = 0;

    public override void Execute(ICpu cpu)
    {        
        if (cpu.State.InstructionSubstate == 0)
        {
            _addr = cpu.FetchMemory();
            cpu.State.InstructionSubstate++;
            if (cpu.State.Halted) return;
        }

        _addr += (ushort)(cpu.FetchMemory() << 8);

        cpu.Registers.PC = _addr;
        cpu.State.ClearInstruction();
        
    }
}