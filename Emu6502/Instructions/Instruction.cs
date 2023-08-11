namespace Emu6502.Instructions;

public abstract class Instruction
{
    protected List<Action<ICpu>>? SubTasks;
    
    public virtual void Execute(ICpu cpu)
    {
        if (SubTasks is null)
            throw new NotImplementedException(
                "Default implementation of Instruction.Execute used but no subtasks where defined.");

        for (
            int i = cpu.State.InstructionSubstate;
            i < SubTasks.Count;
            i++)
        {
            if (cpu.State.InstructionSubstate == i)
            {
                var subtask = SubTasks[i];
                subtask(cpu);

                if (i == SubTasks.Count - 1)
                {
                    cpu.State.ClearInstruction();
                }
                else
                {
                    cpu.State.InstructionSubstate++;
                }

                if (cpu.State.Halted) return;
            }


        }
    }

    protected ushort IndAddr;
    protected ushort Addr;
    
    protected List<Action<ICpu>> AbsoluteAddressing()
    {
        return new()
        {
            (cpu) => { Addr = cpu.FetchMemory(); },
            (cpu) => { Addr += (ushort)(cpu.FetchMemory() << 8); }
        };
    }
}
