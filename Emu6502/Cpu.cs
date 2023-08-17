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
        public const byte LDA_Zeropage =  0xA5;
        public const byte LDA_ZeropageX = 0xB5;
        public const byte LDA_Absolute =  0xAD;
        public const byte LDA_AbsoluteX = 0xBD;
        public const byte LDA_AbsoluteY = 0xB9;
        public const byte LDA_IndirectX = 0xA1;
        public const byte LDA_IndirectY = 0xB1;

        public const byte LDX_Immediate = 0xA2;
        public const byte LDX_Zeropage =  0xA6;
        public const byte LDX_ZeropageY = 0xB6;
        public const byte LDX_Absolute =  0xAE;
        public const byte LDX_AbsoluteY = 0xBE;

        public const byte LDY_Immediate = 0xA0;
        public const byte LDY_Zeropage =  0xA4;
        public const byte LDY_ZeropageX = 0xB4;
        public const byte LDY_Absolute =  0xAC;
        public const byte LDY_AbsoluteX = 0xBC;

        public const byte ADC_Immediate = 0x69;
        public const byte ADC_Zeropage =  0x65;
        public const byte ADC_ZeropageX = 0x75;
        public const byte ADC_Absolute =  0x6D;
        public const byte ADC_AbsoluteX = 0x7D;
        public const byte ADC_AbsoluteY = 0x79;
        public const byte ADC_IndirectX = 0x61;
        public const byte ADC_IndirectY = 0x71;

        public const byte SBC_Immediate = 0xE9;
        public const byte SBC_Zeropage =  0xE5;
        public const byte SBC_ZeropageX = 0xF5;
        public const byte SBC_Absolute =  0xED;
        public const byte SBC_AbsoluteX = 0xFD;
        public const byte SBC_AbsoluteY = 0xF9;
        public const byte SBC_IndirectX = 0xE1;
        public const byte SBC_IndirectY = 0xF1;

        public const byte ORA_Immediate = 0x09;
        public const byte ORA_Zeropage = 0x05;
        public const byte ORA_ZeropageX = 0x15;
        public const byte ORA_Absolute = 0x0D;
        public const byte ORA_AbsoluteX = 0x1D;
        public const byte ORA_AbsoluteY = 0x19;
        public const byte ORA_IndirectX = 0x01;
        public const byte ORA_IndirectY = 0x11;

        public const byte EOR_Immediate = 0x49;
        public const byte EOR_Zeropage =  0x45;
        public const byte EOR_ZeropageX = 0x55;
        public const byte EOR_Absolute =  0x4D;
        public const byte EOR_AbsoluteX = 0x5D;
        public const byte EOR_AbsoluteY = 0x59;
        public const byte EOR_IndirectX = 0x41;
        public const byte EOR_IndirectY = 0x51;

        public const byte AND_Immediate = 0x29;
        public const byte AND_Zeropage =  0x25;
        public const byte AND_ZeropageX = 0x35;
        public const byte AND_Absolute =  0x2D;
        public const byte AND_AbsoluteX = 0x3D;
        public const byte AND_AbsoluteY = 0x39;
        public const byte AND_IndirectX = 0x21;
        public const byte AND_IndirectY = 0x31;

        public const byte DEC_Zeropage =  0xC6;
        public const byte DEC_ZeropageX = 0xD6;
        public const byte DEC_Absolute =  0xCD;
        public const byte DEC_AbsoluteX = 0xDE;
        public const byte DEX = 0xCA;
        public const byte DEY = 0x88;

        public const byte INC_Zeropage =  0xE6;
        public const byte INC_ZeropageX = 0xF6;
        public const byte INC_Absolute =  0xEE;
        public const byte INC_AbsoluteX = 0xFE;
        public const byte INX = 0xE8;
        public const byte INY = 0xC8;

        public const byte NOP = 0xEA;

        public const byte CLC = 0x18;
        public const byte CLD = 0xD8;
        public const byte CLI = 0x58;
        public const byte CLV = 0xB8;

        public const byte SEC = 0x38;
        public const byte SED = 0xF8;
        public const byte SEI = 0x78;

        public const byte PHA = 0x48;
        public const byte PLA = 0x68;
        public const byte PHP = 0x08;
        public const byte PLP = 0x28;

        public const byte TAX = 0xAA;
        public const byte TAY = 0xA8;
        public const byte TXA = 0xBA;
        public const byte TXS = 0x8A;
        public const byte TSX = 0x9A;
        public const byte TYA = 0x98;
    }

    public ExecutionState State { get; } = new();
    public Flags Flags { get; set; } = new();
    public Registers Registers { get; set; } = new();

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

        _instructions[Instructions.LDX_Immediate] = new LDX_Immediate();
        _instructions[Instructions.LDX_Absolute] = new LDX_Absolute();
        _instructions[Instructions.LDX_AbsoluteY] = new LDX_AbsoluteY();
        _instructions[Instructions.LDX_Zeropage] = new LDX_Zeropage();
        _instructions[Instructions.LDX_ZeropageY] = new LDX_ZeropageY();

        _instructions[Instructions.LDY_Immediate] = new LDY_Immediate();
        _instructions[Instructions.LDY_Absolute] = new LDY_Absolute();
        _instructions[Instructions.LDY_AbsoluteX] = new LDY_AbsoluteX();
        _instructions[Instructions.LDY_Zeropage] = new LDY_Zeropage();
        _instructions[Instructions.LDY_ZeropageX] = new LDY_ZeropageX();

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

        _instructions[Instructions.ORA_Immediate] = new ORA_Immediate();
        _instructions[Instructions.ORA_Absolute] = new ORA_Absolute();
        _instructions[Instructions.ORA_AbsoluteX] = new ORA_AbsoluteX();
        _instructions[Instructions.ORA_AbsoluteY] = new ORA_AbsoluteY();
        _instructions[Instructions.ORA_Zeropage] = new ORA_Zeropage();
        _instructions[Instructions.ORA_ZeropageX] = new ORA_ZeropageX();
        _instructions[Instructions.ORA_IndirectX] = new ORA_IndirectX();
        _instructions[Instructions.ORA_IndirectY] = new ORA_IndirectY();

        _instructions[Instructions.EOR_Immediate] = new EOR_Immediate();
        _instructions[Instructions.EOR_Absolute] = new EOR_Absolute();
        _instructions[Instructions.EOR_AbsoluteX] = new EOR_AbsoluteX();
        _instructions[Instructions.EOR_AbsoluteY] = new EOR_AbsoluteY();
        _instructions[Instructions.EOR_Zeropage] = new EOR_Zeropage();
        _instructions[Instructions.EOR_ZeropageX] = new EOR_ZeropageX();
        _instructions[Instructions.EOR_IndirectX] = new EOR_IndirectX();
        _instructions[Instructions.EOR_IndirectY] = new EOR_IndirectY();

        _instructions[Instructions.AND_Immediate] = new AND_Immediate();
        _instructions[Instructions.AND_Absolute] = new AND_Absolute();
        _instructions[Instructions.AND_AbsoluteX] = new AND_AbsoluteX();
        _instructions[Instructions.AND_AbsoluteY] = new AND_AbsoluteY();
        _instructions[Instructions.AND_Zeropage] = new AND_Zeropage();
        _instructions[Instructions.AND_ZeropageX] = new AND_ZeropageX();
        _instructions[Instructions.AND_IndirectX] = new AND_IndirectX();
        _instructions[Instructions.AND_IndirectY] = new AND_IndirectY();

        _instructions[Instructions.DEC_Absolute] = new DEC_Absolute();
        _instructions[Instructions.DEC_AbsoluteX] = new DEC_AbsoluteX();
        _instructions[Instructions.DEC_Zeropage] = new DEC_Zeropage();
        _instructions[Instructions.DEC_ZeropageX] = new DEC_ZeropageX();
        _instructions[Instructions.DEX] = new DEX();
        _instructions[Instructions.DEY] = new DEY();

        _instructions[Instructions.INC_Absolute] = new INC_Absolute();
        _instructions[Instructions.INC_AbsoluteX] = new INC_AbsoluteX();
        _instructions[Instructions.INC_Zeropage] = new INC_Zeropage();
        _instructions[Instructions.INC_ZeropageX] = new INC_ZeropageX();
        _instructions[Instructions.INX] = new INX();
        _instructions[Instructions.INY] = new INY();

        _instructions[Instructions.NOP] = new NOP();

        _instructions[Instructions.CLC] = new CLC();
        _instructions[Instructions.CLD] = new CLD();
        _instructions[Instructions.CLI] = new CLI();
        _instructions[Instructions.CLV] = new CLV();

        _instructions[Instructions.SEC] = new SEC();
        _instructions[Instructions.SED] = new SED();
        _instructions[Instructions.SEI] = new SEI();

        _instructions[Instructions.PHA] = new PHA();
        _instructions[Instructions.PLA] = new PLA();
        _instructions[Instructions.PHP] = new PHP();
        _instructions[Instructions.PLP] = new PLP();

        _instructions[Instructions.TAX] = new TAX();
        _instructions[Instructions.TAY] = new TAY();
        _instructions[Instructions.TXA] = new TXA();
        _instructions[Instructions.TXS] = new TXS();
        _instructions[Instructions.TSX] = new TSX();
        _instructions[Instructions.TYA] = new TYA();

        _instructions[Instructions.Test_2cycle] = new Test_2cycle();
    }

    public void Reset()
    {
        Registers.A = 0;
        Registers.X = 0;
        Registers.Y = 0;
        Registers.SP = 0xFF;
        Flags.N = false;
        Flags.V = false;
        Flags.B = false;
        Flags.D = false;
        Flags.I = false;
        Flags.Z = false;
        Flags.C = false;

        Registers.PC = (ushort)(_memory[0xFFFC] + (_memory[0xFFFD] << 8));
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

    public void WriteMemory(byte value, ushort? addr = null)
    {
        State.Tick();
        _memory[addr ?? Registers.PC++] = value;
    }

    public void SetRegister(Register register, byte value)
    {
        switch (register)
        {
            case Register.A:
                Registers.A = value;
                break;
            case Register.X:
                Registers.X = value;
                break;
            case Register.Y:
                Registers.Y = value;
                break;
            case Register.SP:
                Registers.SP = value;
                break;
        }

        Flags.N = (value & 0x80) > 0;
        Flags.Z = value == 0;
    }
}