using Emu6502.Instructions;

namespace Emu6502;

public class Cpu : ICpu
{
    private byte[] _memory;

    public static class Instructions
    {
        public const byte JMP_Absolute = 0x4C;
        public const byte JMP_Indirect = 0x6C;

        public const byte JSR_Absolute = 0x20;

        public const byte RTI = 0x40;

        public const byte RTS = 0x60;

        public const byte BRK = 0x00;

        public const byte BPL = 0x10;
        public const byte BMI = 0x30;
        public const byte BVC = 0x50;
        public const byte BVS = 0x70;
        public const byte BCC = 0x90;
        public const byte BCS = 0xB0;
        public const byte BNE = 0xD0;
        public const byte BEQ = 0xF0;

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
        public const byte DEC_Absolute =  0xCE;
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

        public const byte STA_Zeropage =  0x85;
        public const byte STA_ZeropageX = 0x95;
        public const byte STA_Absolute =  0x8D;
        public const byte STA_AbsoluteX = 0x9D;
        public const byte STA_AbsoluteY = 0x99;
        public const byte STA_IndirectX = 0x81;
        public const byte STA_IndirectY = 0x91;

        public const byte STX_Zeropage =  0x86;
        public const byte STX_ZeropageY = 0x96;
        public const byte STX_Absolute =  0x8E;

        public const byte STY_Zeropage =  0x84;
        public const byte STY_ZeropageX = 0x94;
        public const byte STY_Absolute =  0x8C;

        public const byte ASL_Accumulator = 0x0A;
        public const byte ASL_Zeropage =    0x06;
        public const byte ASL_ZeropageX =   0x16;
        public const byte ASL_Absolute =    0x0E;
        public const byte ASL_AbsoluteX =   0x1E;

        public const byte LSR_Accumulator = 0x4A;
        public const byte LSR_Zeropage =    0x46;
        public const byte LSR_ZeropageX =   0x56;
        public const byte LSR_Absolute =    0x4E;
        public const byte LSR_AbsoluteX =   0x5E;

        public const byte ROL_Accumulator = 0x2A;
        public const byte ROL_Zeropage =    0x26;
        public const byte ROL_ZeropageX =   0x36;
        public const byte ROL_Absolute =    0x2E;
        public const byte ROL_AbsoluteX =   0x3E;

        public const byte ROR_Accumulator = 0x6A;
        public const byte ROR_Zeropage =    0x66;
        public const byte ROR_ZeropageX =   0x76;
        public const byte ROR_Absolute =    0x6E;
        public const byte ROR_AbsoluteX =   0x7E;

        public const byte CMP_Immediate = 0xC9;
        public const byte CMP_Zeropage =  0xC5;
        public const byte CMP_ZeropageX = 0xD5;
        public const byte CMP_Absolute =  0xCD;
        public const byte CMP_AbsoluteX = 0xDD;
        public const byte CMP_AbsoluteY = 0xD9;
        public const byte CMP_IndirectX = 0xC1;
        public const byte CMP_IndirectY = 0xD1;

        public const byte CPX_Immediate = 0xE0;
        public const byte CPX_Zeropage =  0xE4;
        public const byte CPX_Absolute =  0xEC;

        public const byte CPY_Immediate = 0xC0;
        public const byte CPY_Zeropage =  0xC4;
        public const byte CPY_Absolute =  0xCC;

        public const byte BIT_Zeropage = 0x24;
        public const byte BIT_Absolute = 0x2C;
    }

    public ExecutionState State { get; private set; } = new();
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
        _instructions[Instructions.JMP_Indirect] = new JMP_Indirect();

        _instructions[Instructions.JSR_Absolute] = new JSR_Absolute();

        _instructions[Instructions.RTI] = new RTI();

        _instructions[Instructions.RTS] = new RTS();

        _instructions[Instructions.BRK] = new BRK();

        _instructions[Instructions.BPL] = new BPL();
        _instructions[Instructions.BMI] = new BMI();
        _instructions[Instructions.BVC] = new BVC();
        _instructions[Instructions.BVS] = new BVS();
        _instructions[Instructions.BCC] = new BCC();
        _instructions[Instructions.BCS] = new BCS();
        _instructions[Instructions.BNE] = new BNE();
        _instructions[Instructions.BEQ] = new BEQ();

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

        _instructions[Instructions.STA_Zeropage] = new STA_Zeropage();
        _instructions[Instructions.STA_ZeropageX] = new STA_ZeropageX();
        _instructions[Instructions.STA_Absolute] = new STA_Absolute();
        _instructions[Instructions.STA_AbsoluteX] = new STA_AbsoluteX();
        _instructions[Instructions.STA_AbsoluteY] = new STA_AbsoluteY();
        _instructions[Instructions.STA_IndirectX] = new STA_IndirectX();
        _instructions[Instructions.STA_IndirectY] = new STA_IndirectY();

        _instructions[Instructions.STX_Zeropage] = new STX_Zeropage();
        _instructions[Instructions.STX_ZeropageY] = new STX_ZeropageY();
        _instructions[Instructions.STX_Absolute] = new STX_Absolute();

        _instructions[Instructions.STY_Zeropage] = new STY_Zeropage();
        _instructions[Instructions.STY_ZeropageX] = new STY_ZeropageX();
        _instructions[Instructions.STY_Absolute] = new STY_Absolute();

        _instructions[Instructions.ASL_Accumulator] = new ASL_Accumulator();
        _instructions[Instructions.ASL_Zeropage] = new ASL_Zeropage();
        _instructions[Instructions.ASL_ZeropageX] = new ASL_ZeropageX();
        _instructions[Instructions.ASL_Absolute] = new ASL_Absolute();
        _instructions[Instructions.ASL_AbsoluteX] = new ASL_AbsoluteX();

        _instructions[Instructions.LSR_Accumulator] = new LSR_Accumulator();
        _instructions[Instructions.LSR_Zeropage] = new LSR_Zeropage();
        _instructions[Instructions.LSR_ZeropageX] = new LSR_ZeropageX();
        _instructions[Instructions.LSR_Absolute] = new LSR_Absolute();
        _instructions[Instructions.LSR_AbsoluteX] = new LSR_AbsoluteX();

        _instructions[Instructions.ROL_Accumulator] = new ROL_Accumulator();
        _instructions[Instructions.ROL_Zeropage] = new ROL_Zeropage();
        _instructions[Instructions.ROL_ZeropageX] = new ROL_ZeropageX();
        _instructions[Instructions.ROL_Absolute] = new ROL_Absolute();
        _instructions[Instructions.ROL_AbsoluteX] = new ROL_AbsoluteX();

        _instructions[Instructions.ROR_Accumulator] = new ROR_Accumulator();
        _instructions[Instructions.ROR_Zeropage] = new ROR_Zeropage();
        _instructions[Instructions.ROR_ZeropageX] = new ROR_ZeropageX();
        _instructions[Instructions.ROR_Absolute] = new ROR_Absolute();
        _instructions[Instructions.ROR_AbsoluteX] = new ROR_AbsoluteX();

        _instructions[Instructions.CMP_Immediate] = new CMP_Immediate();
        _instructions[Instructions.CMP_Zeropage] = new CMP_Zeropage();
        _instructions[Instructions.CMP_ZeropageX] = new CMP_ZeropageX();
        _instructions[Instructions.CMP_Absolute] = new CMP_Absolute();
        _instructions[Instructions.CMP_AbsoluteX] = new CMP_AbsoluteX();
        _instructions[Instructions.CMP_AbsoluteY] = new CMP_AbsoluteY();
        _instructions[Instructions.CMP_IndirectX] = new CMP_IndirectX();
        _instructions[Instructions.CMP_IndirectY] = new CMP_IndirectY();

        _instructions[Instructions.CPX_Immediate] = new CPX_Immediate();
        _instructions[Instructions.CPX_Zeropage] = new CPX_Zeropage();
        _instructions[Instructions.CPX_Absolute] = new CPX_Absolute();

        _instructions[Instructions.CPY_Immediate] = new CPY_Immediate();
        _instructions[Instructions.CPY_Zeropage] = new CPY_Zeropage();
        _instructions[Instructions.CPY_Absolute] = new CPY_Absolute();

        _instructions[Instructions.BIT_Zeropage] = new BIT_Zeropage();
        _instructions[Instructions.BIT_Absolute] = new BIT_Absolute();
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
        Flags.I = true;
        Flags.Z = false;
        Flags.C = false;

        Registers.PC = (ushort)(_memory[0xFFFC] + (_memory[0xFFFD] << 8));

        State = new();
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

        UpdateNZ(value);
    }

    public void UpdateNZ(byte value)
    {
        Flags.N = (value & 0x80) > 0;
        Flags.Z = value == 0;
    }
}