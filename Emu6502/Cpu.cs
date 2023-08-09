using Emu6502.Instructions;

namespace Emu6502;

public class Cpu : ICpu
{
    private byte[] _memory;

    public static class Instructions
    {
        public const byte Test_2cycle = 0x02;
        public const byte LDA_Immediate = 0xA9;
        public const byte NOP = 0xEA;
    }

    public ExecutionState State { get; } = new();
    public Flags Flags { get; } = new();
    public Registers Registers { get; } = new();

    public int Ticks => State.Ticks;

    private Instruction[] _instructions = new Instruction[256];

    public Cpu(byte[] memory)
    {
        SetupInstructionsTable();

        _memory = memory;
    }

    private void SetupInstructionsTable()
    {
        Array.Fill(_instructions, new InvalidOperation());
        _instructions[Instructions.LDA_Immediate] = new LDA_Immediate();

        _instructions[Instructions.Test_2cycle] = new Test_2cycle();
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
        State.RemainingCycles += cycles;

        while (!State.Halted)
        {
            if (State.Instruction is null)
            {
                var inst = FetchMemory();
                try
                {
                    State.Instruction = _instructions[inst];
                }
                catch (KeyNotFoundException)
                {
                    Console.WriteLine($"Error: Undefined instruction 0x{inst:X2} @0x{Registers.PC:X4}");
                    break;
                }
            }

            if (State.Halted) return;
            State.Instruction.Execute(this);
        }
    }

    public byte FetchMemory()
    {
        var b = _memory[Registers.PC];
        Registers.PC++;
        State.Tick();
        return b;
    }
}