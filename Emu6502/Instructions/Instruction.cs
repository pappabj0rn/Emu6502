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

                if (cpu.State.InstructionSubstate == SubTasks.Count - 1)
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
    protected ushort? Addr;

    protected List<Action<ICpu>> AbsoluteAddressing()
    {
        return new()
        {
            (cpu) => { Addr = cpu.FetchMemory(); },
            (cpu) => { Addr += (ushort)(cpu.FetchMemory() << 8); }
        };
    }

    protected List<Action<ICpu>> AbsoluteXAddressing(bool addCyclePenalty = false)
    {
        return new()
        {
            (cpu) => { Addr = (ushort)(cpu.FetchMemory() + cpu.Registers.X); },
            (cpu) => {
                if(addCyclePenalty 
                   || Addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => { Addr += (ushort)(cpu.FetchMemory() << 8); }
        };
    }

    protected List<Action<ICpu>> AbsoluteYAddressing(bool addCyclePenalty = false)
    {
        return new()
        {
            (cpu) => { Addr = (ushort)(cpu.FetchMemory() + cpu.Registers.Y); },
            (cpu) => {
                if(addCyclePenalty
                   || Addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => { Addr += (ushort)(cpu.FetchMemory() << 8); }
        };
    }

    protected List<Action<ICpu>> ZeropageAddressing()
    {
        return new()
        {
            (cpu) => { Addr = cpu.FetchMemory(); }
        };
    }

    protected List<Action<ICpu>> ZeropageXAddressing()
    {
        return new()
        {
            (cpu) => { Addr = cpu.FetchMemory(); },
            (cpu) => {
                Addr += cpu.Registers.X;
                Addr &= 0xff;
                cpu.State.Tick();
            }
        };
    }

    protected List<Action<ICpu>> ZeropageYAddressing()
    {
        return new()
        {
            (cpu) => { Addr = cpu.FetchMemory(); },
            (cpu) => {
                Addr += cpu.Registers.Y;
                Addr &= 0xff;
                cpu.State.Tick();
            }
        };
    }

    protected List<Action<ICpu>> IndirectXAdressing()
    {
        return new()
        {
            (cpu) => { IndAddr = cpu.FetchMemory(); },
            (cpu) => {
                IndAddr += cpu.Registers.X;
                cpu.State.Tick();
            },
            (cpu) => { Addr = cpu.FetchMemory((ushort)(IndAddr & 0x00ff)); },
            (cpu) => { Addr += (ushort)(cpu.FetchMemory((ushort)((IndAddr + 1) & 0x00ff)) << 8); }
        };
    }

    protected List<Action<ICpu>> IndirectYAdressing(bool addCyclePenalty = false)
    {
        return new()
        {
            (cpu) => { IndAddr = cpu.FetchMemory(); },
            (cpu) => { Addr = (ushort)(cpu.FetchMemory(IndAddr) + cpu.Registers.Y); },
            (cpu) => {
                if(addCyclePenalty
                   || Addr > 0xff)
                {
                    cpu.State.Tick();
                }
            },
            (cpu) => { Addr += (ushort)(cpu.FetchMemory((ushort?)(IndAddr + 1)) << 8); }
        };
    }
}
