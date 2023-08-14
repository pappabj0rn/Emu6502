using Emu6502.Instructions;

namespace Emu6502;

public class Cpu : ICpu
{
    private byte[] _memory;

    public static class Instructions
    {
        public const byte Test_2cycle = 0x02;
        public const byte JMP_Absolute = 0x5C;

        public const byte LDA_Immediate = 0xA9;
        public const byte LDA_Zeropage = 0xA5;
        public const byte LDA_ZeropageX = 0xB5;
        public const byte LDA_Absolute = 0xAD;
        public const byte LDA_AbsoluteX = 0xBD;
        public const byte LDA_AbsoluteY = 0xB9;
        public const byte LDA_IndirectX = 0xA1;
        public const byte LDA_IndirectY = 0xB1;

        public const byte ADC_Immediate = 0x69;
        public const byte ADC_Zeropage = 0x65;
        public const byte ADC_ZeropageX = 0x75;
        public const byte ADC_Absolute = 0x6D;
        public const byte ADC_AbsoluteX = 0x7D;
        public const byte ADC_AbsoluteY = 0x79;
        public const byte ADC_IndirectX = 0x61;
        public const byte ADC_IndirectY = 0x71;

        public const byte SBC_Immediate = 0xE9;
        public const byte SBC_Zeropage = 0xE5;
        public const byte SBC_ZeropageX = 0xF5;
        public const byte SBC_Absolute = 0xED;
        public const byte SBC_AbsoluteX = 0xFD;
        public const byte SBC_AbsoluteY = 0xF9;
        public const byte SBC_IndirectX = 0xE1;
        public const byte SBC_IndirectY = 0xF1;

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
        _instructions[Instructions.JMP_Absolute] = new JMP_Absolute();

        _instructions[Instructions.LDA_Immediate] = new LDA_Immediate();
        _instructions[Instructions.LDA_Absolute] = new LDA_Absolute();
        _instructions[Instructions.LDA_AbsoluteX] = new LDA_AbsoluteX();
        _instructions[Instructions.LDA_AbsoluteY] = new LDA_AbsoluteY();
        _instructions[Instructions.LDA_Zeropage] = new LDA_Zeropage();
        _instructions[Instructions.LDA_ZeropageX] = new LDA_ZeropageX();
        _instructions[Instructions.LDA_IndirectX] = new LDA_IndirectX();
        _instructions[Instructions.LDA_IndirectY] = new LDA_IndirectY();

        _instructions[Instructions.ADC_Immediate] = new ADC_Immediate();
        _instructions[Instructions.ADC_Absolute] = new ADC_Absolute();
        _instructions[Instructions.ADC_AbsoluteX] = new ADC_AbsoluteX();
        _instructions[Instructions.ADC_AbsoluteY] = new ADC_AbsoluteY();
        _instructions[Instructions.ADC_Zeropage] = new ADC_Zeropage();
        _instructions[Instructions.ADC_ZeropageX] = new ADC_ZeropageX();
        _instructions[Instructions.ADC_IndirectX] = new ADC_IndirectX();
        _instructions[Instructions.ADC_IndirectY] = new ADC_IndirectY();

        _instructions[Instructions.SBC_Immediate] = new SBC_Immediate();
        _instructions[Instructions.SBC_Absolute] = new SBC_Absolute();
        _instructions[Instructions.SBC_AbsoluteX] = new SBC_AbsoluteX();
        _instructions[Instructions.SBC_AbsoluteY] = new SBC_AbsoluteY();
        _instructions[Instructions.SBC_Zeropage] = new SBC_Zeropage();
        _instructions[Instructions.SBC_ZeropageX] = new SBC_ZeropageX();
        _instructions[Instructions.SBC_IndirectX] = new SBC_IndirectX();
        _instructions[Instructions.SBC_IndirectY] = new SBC_IndirectY();

        _instructions[Instructions.NOP] = new NOP();

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
                State.Instruction = _instructions[inst];
            }

            if (State.Halted) return;
            State.Instruction.Execute(this);
        }
    }

    public byte FetchMemory(ushort? addr = null)
    {
        State.Tick();
        return _memory[addr ?? Registers.PC++];
    }
}