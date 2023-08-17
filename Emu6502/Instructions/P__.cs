namespace Emu6502.Instructions;

public class PHA : Instruction
{
    public PHA()
    {        
        SubTasks = new()
        {
            (cpu) => { cpu.State.Tick(); },
            (cpu) => {
                cpu.WriteMemory(cpu.Registers.A, (ushort?)(0x0100 | cpu.Registers.SP));
                cpu.Registers.SP--;
            }
        };
    }
}

public class PLA : Instruction
{
    public PLA()
    {
        SubTasks = new()
        {
            (cpu) => { cpu.State.Tick(); },
            (cpu) => {
                cpu.Registers.SP++;
                cpu.State.Tick();
            },
            (cpu) => {
                cpu.SetRegister(Register.A, cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP)));
            }
        };
    }
}

public class PHP : Instruction
{
    public PHP()
    {
        SubTasks = new()
        {
            (cpu) => { cpu.State.Tick(); },
            (cpu) => {
                cpu.WriteMemory(cpu.Flags.GetSR(), (ushort?)(0x0100 | cpu.Registers.SP));
                cpu.Registers.SP--;
            }
        };
    }
}

public class PLP : Instruction
{
    public PLP()
    {
        SubTasks = new()
        {
            (cpu) => { cpu.State.Tick(); },
            (cpu) => {
                cpu.Registers.SP++;
                cpu.State.Tick();
            },
            (cpu) => {
                var mem = cpu.FetchMemory((ushort?)(0x0100 | cpu.Registers.SP));
                cpu.Flags.SetSR((byte)(0xCF & mem));
            }
        };
    }
}
