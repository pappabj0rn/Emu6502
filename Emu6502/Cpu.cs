namespace Emu6502;

public class Registers
{
    public byte A;
    public byte X;
    public byte Y;
    public byte S;
    public ushort PC;
}

public class Flags
{
    public bool N;
    public bool V;
    public bool B;
    public bool D;
    public bool I;
    public bool Z;
    public bool C;
}

internal class ExecutionState
{
    internal int RemainingCycles;
    internal int Ticks;
    
    internal Action<Cpu>? Instruction;
    internal int InstructionSubstate;

    internal bool Halted => RemainingCycles == 0;

    internal void Tick()
    {
        RemainingCycles--;
        Ticks++;
    }

    internal void ClearInstruction()
    {
        Instruction = null;
        InstructionSubstate = 0;
    }
}

public class Cpu
{
    private ExecutionState _state = new();
    private byte[] _memory;

    public static class Instructions
    {
        public const byte Test_2cycle = 0x02;
        public const byte LDA_Immediate = 0xA9;
        public const byte NOP = 0xEA;
    }

    public Flags Flags { get; } = new();
    public Registers Registers { get; } = new();

    public int Ticks => _state.Ticks;

    private Dictionary<byte, Action<Cpu>> _instructions = new()
    {
#if DEBUG
        {
            Instructions.Test_2cycle,
            (cpu) =>
            {
                if(cpu._state.InstructionSubstate == 0)
                {
                    Console.WriteLine("test instruction cycle 1");
                    cpu.Registers.X = 1;
                    cpu._state.Tick();
                    cpu._state.InstructionSubstate++; 
                }

                if(cpu._state.Halted) return;
                Console.WriteLine("test instruction cycle 2");
                cpu.Registers.X = 2;
                cpu._state.Tick();

                cpu._state.ClearInstruction();
            }
        },
#endif
        {
            Instructions.LDA_Immediate,
            (cpu) =>
            {
                cpu.Registers.A = cpu.FetchMemory();
                cpu.Flags.N = (cpu.Registers.A & 0b1000_0000) > 0;
                cpu.Flags.Z = cpu.Registers.A == 0;

                cpu._state.ClearInstruction();
            }
        },
        //{
        //    Instructions.NOP,
        //    (cpu, state) =>
        //    {
        //        cpu.Ticks++;
        //    }
        //},
    };

    public Cpu(byte[] memory)
    {
        _memory = memory;
    }

    public void Reset()
    {
        Registers.A = 0;
        Registers.X = 0;
        Registers.Y = 0;
        Registers.S = 0;
        Flags.N = false;
        Flags.V = false;
        Flags.B = false;
        Flags.D = false;
        Flags.I = false;
        Flags.Z = false;
        Flags.C = false;

        Registers.PC = (ushort)(_memory[0xfffc] + (_memory[0xfffd] << 8));
    }

    public void Execute(int cycles)
    {
        _state.RemainingCycles += cycles;

        while (!_state.Halted)
        {
            if(_state.Instruction is null)
            {
                var inst = FetchMemory();
                try
                {
                    _state.Instruction = _instructions[inst];
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine($"Error: Undefined instruction 0x{inst:X2} @0x{Registers.PC:X4}");
                    break;
                }                
            }

            if (_state.Halted) return;
            _state.Instruction(this);
        }
    }

    private byte FetchMemory()
    {
        var b = _memory[Registers.PC];
        Registers.PC++;
        _state.Tick();
        return b;
    }
}