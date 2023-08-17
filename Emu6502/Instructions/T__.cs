namespace Emu6502.Instructions;

public class TAX : Instruction
{
    public TAX()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.X, cpu.Registers.A);
                cpu.State.Tick();
            }
        };
    }
}

public class TAY : Instruction
{
    public TAY()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.Y, cpu.Registers.A);
                cpu.State.Tick();
            }
        };
    }
}

public class TSX : Instruction
{
    public TSX()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.X, cpu.Registers.SP);
                cpu.State.Tick();
            }
        };
    }
}

public class TXA : Instruction
{
    public TXA()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.A, cpu.Registers.X);
                cpu.State.Tick();
            }
        };
    }
}

public class TXS : Instruction
{
    public TXS()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.SP, cpu.Registers.X);
                cpu.State.Tick();
            }
        };
    }
}

public class TYA : Instruction
{
    public TYA()
    {
        SubTasks = new()
        {
            (cpu) => {
                cpu.SetRegister(Register.A, cpu.Registers.Y);
                cpu.State.Tick();
            }
        };
    }
}
