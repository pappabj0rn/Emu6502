using System.Text;

namespace Emu6502.Tests.Unit;

public abstract class CpuTests
{
    private Cpu Cpu;
    private byte[] Memory;

    public CpuTests()
    {
        Memory = new byte[0x10000];
        Cpu = new Cpu(Memory);
    }

    public class Reset : CpuTests
    {
        [Fact]
        public void Should_initialize_flags_and_registers_and_load_reset_vector_into_pc()
        {
            Cpu.Reset();

            Cpu.Flags.N.Should().BeFalse();
            Cpu.Flags.V.Should().BeFalse();
            Cpu.Flags.B.Should().BeFalse();
            Cpu.Flags.D.Should().BeFalse();
            Cpu.Flags.I.Should().BeTrue();
            Cpu.Flags.Z.Should().BeFalse();
            Cpu.Flags.C.Should().BeFalse();

            Cpu.Registers.A.Should().Be(0x00);
            Cpu.Registers.X.Should().Be(0x00);
            Cpu.Registers.Y.Should().Be(0x00);
            Cpu.Registers.SP.Should().Be(0xFF);
        }

        [Theory]
        [InlineData(0x00, 0x00, 0x0000)]
        [InlineData(0x02, 0x01, 0x0102)]
        [InlineData(0x01, 0x02, 0x0201)]
        public void Should_load_reset_vector_into_pc(byte low, byte high, ushort expected)
        {
            Memory[0xfffc] = low;
            Memory[0xfffd] = high;
            Cpu.Reset();

            Cpu.Registers.PC.Should().Be(expected);
        }
    }

    public class Execute : CpuTests
    {
        public Execute()
        {
            Cpu.Reset();
        }

        [Fact]
        public void Should_be_able_to_execute_multi_cycle_instructions_using_multiple_calls_to_execute()
        {
            Memory[0x00] = Cpu.Instructions.LDA_Immediate;
            Memory[0x01] = 1;

            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(1);
            Cpu.Registers.PC.Should().Be(0x01);
            Cpu.Registers.A.Should().Be(0);

            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(2);
            Cpu.Registers.PC.Should().Be(0x02);
            Cpu.Registers.A.Should().Be(1);
        }

        [Fact]
        public void Should_be_able_to_resume_multi_cycle_instructions_using_multiple_calls_to_execute()
        {
            Memory[0x0000] = Cpu.Instructions.LDA_Absolute;
            Memory[0x0001] = 0x03;
            Memory[0x0002] = 0x04;

            Memory[0x0403] = 0x01;

            //fetch
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(1);
            Cpu.Registers.A.Should().Be(0x00);
            Cpu.Registers.PC.Should().Be(0x01);

            //cycle 1
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(2);
            Cpu.Registers.A.Should().Be(0x00);
            Cpu.Registers.PC.Should().Be(0x02);

            //cycle 2
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(3);
            Cpu.Registers.A.Should().Be(0x00);
            Cpu.Registers.PC.Should().Be(0x03);

            //cycle 3
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(4);
            Cpu.Registers.A.Should().Be(0x01);
            Cpu.Registers.PC.Should().Be(0x03);
        }

        [Fact]
        public void Executing_and_invalid_op_code_will_throw_an_exception()
        {
            Memory[0x00] = 0xFF;
            try
            {
                Cpu.Execute(2);
            }
            catch (InvalidOperationException) { }
        }

        [Theory]
        [MemberData(nameof(ExpectedCpuInstructions))]
        public void Should_not_throw_IOE_for_valid_instruction(byte instruction, Type expectedType)
        {
            Memory[0x00] = instruction;
            Cpu.Execute(1);

            Cpu.State.Instruction.Should().BeOfType(expectedType);
        }

        public static IEnumerable<object[]> ExpectedCpuInstructions() 
            => new List<object[]>
            {
                new object[]{ Cpu.Instructions.INC_Zeropage, typeof(INC_Zeropage ) },
                new object[]{ Cpu.Instructions.INC_ZeropageX, typeof(INC_ZeropageX) },
                new object[]{ Cpu.Instructions.INC_Absolute , typeof(INC_Absolute ) },
                new object[]{ Cpu.Instructions.INC_AbsoluteX, typeof(INC_AbsoluteX) },
                new object[]{ Cpu.Instructions.INX, typeof(INX) },
                new object[]{ Cpu.Instructions.INY, typeof(INY) },
                new object[]{ Cpu.Instructions.NOP, typeof(NOP) },
                new object[]{ Cpu.Instructions.CLC, typeof(CLC) },
                new object[]{ Cpu.Instructions.CLD, typeof(CLD) },
                new object[]{ Cpu.Instructions.CLI, typeof(CLI) },
                new object[]{ Cpu.Instructions.CLV, typeof(CLV) },
                new object[]{ Cpu.Instructions.SEC, typeof(SEC) },
                new object[]{ Cpu.Instructions.SED, typeof(SED) },
                new object[]{ Cpu.Instructions.SEI, typeof(SEI) },
                new object[]{ Cpu.Instructions.PHA, typeof(PHA) },
                new object[]{ Cpu.Instructions.PLA, typeof(PLA) },
                new object[]{ Cpu.Instructions.PHP, typeof(PHP) },
                new object[]{ Cpu.Instructions.PLP, typeof(PLP) },
                new object[]{ Cpu.Instructions.TAX, typeof(TAX) },
                new object[]{ Cpu.Instructions.TAY, typeof(TAY) },
                new object[]{ Cpu.Instructions.TXA, typeof(TXA) },
                new object[]{ Cpu.Instructions.TXS, typeof(TXS) },
                new object[]{ Cpu.Instructions.TSX, typeof(TSX) },
                new object[]{ Cpu.Instructions.TYA, typeof(TYA) },
                new object[]{ Cpu.Instructions.RTS, typeof(RTS) },
                new object[]{ Cpu.Instructions.RTI, typeof(RTI) },
                new object[]{ Cpu.Instructions.BRK, typeof(BRK) },
                new object[]{ Cpu.Instructions.BPL, typeof(BPL) },
                new object[]{ Cpu.Instructions.BMI, typeof(BMI) },
                new object[]{ Cpu.Instructions.BVC, typeof(BVC) },
                new object[]{ Cpu.Instructions.BVS, typeof(BVS) },
                new object[]{ Cpu.Instructions.BCC, typeof(BCC) },
                new object[]{ Cpu.Instructions.BCS, typeof(BCS) },
                new object[]{ Cpu.Instructions.BNE, typeof(BNE) },
                new object[]{ Cpu.Instructions.BEQ, typeof(BEQ) },
                new object[]{ Cpu.Instructions.BIT_Zeropage, typeof(BIT_Zeropage) },
                new object[]{ Cpu.Instructions.BIT_Absolute, typeof(BIT_Absolute) },

                new object[]{ Cpu.Instructions.LDA_Immediate, typeof(LDA_Immediate  ) },
                new object[]{ Cpu.Instructions.LDA_Absolute   , typeof(LDA_Absolute   ) },
                new object[]{ Cpu.Instructions.LDA_AbsoluteX  , typeof(LDA_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.LDA_AbsoluteY  , typeof(LDA_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.LDA_Zeropage   , typeof(LDA_Zeropage   ) },
                new object[]{ Cpu.Instructions.LDA_ZeropageX  , typeof(LDA_ZeropageX  ) },
                new object[]{ Cpu.Instructions.LDA_IndirectX  , typeof(LDA_IndirectX  ) },
                new object[]{ Cpu.Instructions.LDA_IndirectY  , typeof(LDA_IndirectY  ) },
                new object[]{ Cpu.Instructions.LDX_Immediate  , typeof(LDX_Immediate  ) },
                new object[]{ Cpu.Instructions.LDX_Absolute   , typeof(LDX_Absolute   ) },
                new object[]{ Cpu.Instructions.LDX_AbsoluteY  , typeof(LDX_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.LDX_Zeropage   , typeof(LDX_Zeropage   ) },
                new object[]{ Cpu.Instructions.LDX_ZeropageY  , typeof(LDX_ZeropageY  ) },
                new object[]{ Cpu.Instructions.LDY_Immediate  , typeof(LDY_Immediate  ) },
                new object[]{ Cpu.Instructions.LDY_Absolute   , typeof(LDY_Absolute   ) },
                new object[]{ Cpu.Instructions.LDY_AbsoluteX  , typeof(LDY_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.LDY_Zeropage   , typeof(LDY_Zeropage   ) },
                new object[]{ Cpu.Instructions.LDY_ZeropageX  , typeof(LDY_ZeropageX  ) },
                new object[]{ Cpu.Instructions.ADC_Immediate  , typeof(ADC_Immediate  ) },
                new object[]{ Cpu.Instructions.ADC_Absolute   , typeof(ADC_Absolute   ) },
                new object[]{ Cpu.Instructions.ADC_AbsoluteX  , typeof(ADC_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.ADC_AbsoluteY  , typeof(ADC_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.ADC_Zeropage   , typeof(ADC_Zeropage   ) },
                new object[]{ Cpu.Instructions.ADC_ZeropageX  , typeof(ADC_ZeropageX  ) },
                new object[]{ Cpu.Instructions.ADC_IndirectX  , typeof(ADC_IndirectX  ) },
                new object[]{ Cpu.Instructions.ADC_IndirectY  , typeof(ADC_IndirectY  ) },
                new object[]{ Cpu.Instructions.SBC_Immediate  , typeof(SBC_Immediate  ) },
                new object[]{ Cpu.Instructions.SBC_Absolute   , typeof(SBC_Absolute   ) },
                new object[]{ Cpu.Instructions.SBC_AbsoluteX  , typeof(SBC_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.SBC_AbsoluteY  , typeof(SBC_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.SBC_Zeropage   , typeof(SBC_Zeropage   ) },
                new object[]{ Cpu.Instructions.SBC_ZeropageX  , typeof(SBC_ZeropageX  ) },
                new object[]{ Cpu.Instructions.SBC_IndirectX  , typeof(SBC_IndirectX  ) },
                new object[]{ Cpu.Instructions.SBC_IndirectY  , typeof(SBC_IndirectY  ) },
                new object[]{ Cpu.Instructions.ORA_Immediate  , typeof(ORA_Immediate  ) },
                new object[]{ Cpu.Instructions.ORA_Absolute   , typeof(ORA_Absolute   ) },
                new object[]{ Cpu.Instructions.ORA_AbsoluteX  , typeof(ORA_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.ORA_AbsoluteY  , typeof(ORA_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.ORA_Zeropage   , typeof(ORA_Zeropage   ) },
                new object[]{ Cpu.Instructions.ORA_ZeropageX  , typeof(ORA_ZeropageX  ) },
                new object[]{ Cpu.Instructions.ORA_IndirectX  , typeof(ORA_IndirectX  ) },
                new object[]{ Cpu.Instructions.ORA_IndirectY  , typeof(ORA_IndirectY  ) },
                new object[]{ Cpu.Instructions.EOR_Immediate  , typeof(EOR_Immediate  ) },
                new object[]{ Cpu.Instructions.EOR_Absolute   , typeof(EOR_Absolute   ) },
                new object[]{ Cpu.Instructions.EOR_AbsoluteX  , typeof(EOR_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.EOR_AbsoluteY  , typeof(EOR_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.EOR_Zeropage   , typeof(EOR_Zeropage   ) },
                new object[]{ Cpu.Instructions.EOR_ZeropageX  , typeof(EOR_ZeropageX  ) },
                new object[]{ Cpu.Instructions.EOR_IndirectX  , typeof(EOR_IndirectX  ) },
                new object[]{ Cpu.Instructions.EOR_IndirectY  , typeof(EOR_IndirectY  ) },
                new object[]{ Cpu.Instructions.AND_Immediate  , typeof(AND_Immediate  ) },
                new object[]{ Cpu.Instructions.AND_Absolute   , typeof(AND_Absolute   ) },
                new object[]{ Cpu.Instructions.AND_AbsoluteX  , typeof(AND_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.AND_AbsoluteY  , typeof(AND_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.AND_Zeropage   , typeof(AND_Zeropage   ) },
                new object[]{ Cpu.Instructions.AND_ZeropageX  , typeof(AND_ZeropageX  ) },
                new object[]{ Cpu.Instructions.AND_IndirectX  , typeof(AND_IndirectX  ) },
                new object[]{ Cpu.Instructions.AND_IndirectY  , typeof(AND_IndirectY  ) },
                new object[]{ Cpu.Instructions.DEC_Zeropage   , typeof(DEC_Zeropage   ) },
                new object[]{ Cpu.Instructions.DEC_ZeropageX  , typeof(DEC_ZeropageX  ) },
                new object[]{ Cpu.Instructions.DEC_Absolute   , typeof(DEC_Absolute   ) },
                new object[]{ Cpu.Instructions.DEC_AbsoluteX  , typeof(DEC_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.DEX            , typeof(DEX            ) },
                new object[]{ Cpu.Instructions.DEY            , typeof(DEY            ) },
                new object[]{ Cpu.Instructions.STA_Zeropage   , typeof(STA_Zeropage   ) },
                new object[]{ Cpu.Instructions.STA_ZeropageX  , typeof(STA_ZeropageX  ) },
                new object[]{ Cpu.Instructions.STA_Absolute   , typeof(STA_Absolute   ) },
                new object[]{ Cpu.Instructions.STA_AbsoluteX  , typeof(STA_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.STA_AbsoluteY  , typeof(STA_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.STA_IndirectX  , typeof(STA_IndirectX  ) },
                new object[]{ Cpu.Instructions.STA_IndirectY  , typeof(STA_IndirectY  ) },
                new object[]{ Cpu.Instructions.STX_Zeropage   , typeof(STX_Zeropage   ) },
                new object[]{ Cpu.Instructions.STX_ZeropageY  , typeof(STX_ZeropageY  ) },
                new object[]{ Cpu.Instructions.STX_Absolute   , typeof(STX_Absolute   ) },
                new object[]{ Cpu.Instructions.STY_Zeropage   , typeof(STY_Zeropage   ) },
                new object[]{ Cpu.Instructions.STY_ZeropageX  , typeof(STY_ZeropageX  ) },
                new object[]{ Cpu.Instructions.STY_Absolute   , typeof(STY_Absolute   ) },
                new object[]{ Cpu.Instructions.ASL_Accumulator, typeof(ASL_Accumulator) },
                new object[]{ Cpu.Instructions.ASL_Zeropage   , typeof(ASL_Zeropage   ) },
                new object[]{ Cpu.Instructions.ASL_ZeropageX  , typeof(ASL_ZeropageX  ) },
                new object[]{ Cpu.Instructions.ASL_Absolute   , typeof(ASL_Absolute   ) },
                new object[]{ Cpu.Instructions.ASL_AbsoluteX  , typeof(ASL_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.LSR_Accumulator, typeof(LSR_Accumulator) },
                new object[]{ Cpu.Instructions.LSR_Zeropage   , typeof(LSR_Zeropage   ) },
                new object[]{ Cpu.Instructions.LSR_ZeropageX  , typeof(LSR_ZeropageX  ) },
                new object[]{ Cpu.Instructions.LSR_Absolute   , typeof(LSR_Absolute   ) },
                new object[]{ Cpu.Instructions.LSR_AbsoluteX  , typeof(LSR_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.ROL_Accumulator, typeof(ROL_Accumulator) },
                new object[]{ Cpu.Instructions.ROL_Zeropage   , typeof(ROL_Zeropage   ) },
                new object[]{ Cpu.Instructions.ROL_ZeropageX  , typeof(ROL_ZeropageX  ) },
                new object[]{ Cpu.Instructions.ROL_Absolute   , typeof(ROL_Absolute   ) },
                new object[]{ Cpu.Instructions.ROL_AbsoluteX  , typeof(ROL_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.ROR_Accumulator, typeof(ROR_Accumulator) },
                new object[]{ Cpu.Instructions.ROR_Zeropage   , typeof(ROR_Zeropage   ) },
                new object[]{ Cpu.Instructions.ROR_ZeropageX  , typeof(ROR_ZeropageX  ) },
                new object[]{ Cpu.Instructions.ROR_Absolute   , typeof(ROR_Absolute   ) },
                new object[]{ Cpu.Instructions.ROR_AbsoluteX  , typeof(ROR_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.CMP_Immediate  , typeof(CMP_Immediate  ) },
                new object[]{ Cpu.Instructions.CMP_Zeropage   , typeof(CMP_Zeropage   ) },
                new object[]{ Cpu.Instructions.CMP_ZeropageX  , typeof(CMP_ZeropageX  ) },
                new object[]{ Cpu.Instructions.CMP_Absolute   , typeof(CMP_Absolute   ) },
                new object[]{ Cpu.Instructions.CMP_AbsoluteX  , typeof(CMP_AbsoluteX  ) },
                new object[]{ Cpu.Instructions.CMP_AbsoluteY  , typeof(CMP_AbsoluteY  ) },
                new object[]{ Cpu.Instructions.CMP_IndirectX  , typeof(CMP_IndirectX  ) },
                new object[]{ Cpu.Instructions.CMP_IndirectY  , typeof(CMP_IndirectY  ) },
                new object[]{ Cpu.Instructions.CPX_Immediate  , typeof(CPX_Immediate  ) },
                new object[]{ Cpu.Instructions.CPX_Zeropage   , typeof(CPX_Zeropage   ) },
                new object[]{ Cpu.Instructions.CPX_Absolute   , typeof(CPX_Absolute   ) },
                new object[]{ Cpu.Instructions.CPY_Immediate  , typeof(CPY_Immediate  ) },
                new object[]{ Cpu.Instructions.CPY_Zeropage   , typeof(CPY_Zeropage   ) },
                new object[]{ Cpu.Instructions.CPY_Absolute   , typeof(CPY_Absolute   ) },
                new object[]{ Cpu.Instructions.JMP_Absolute   , typeof(JMP_Absolute   ) },
                new object[]{ Cpu.Instructions.JMP_Indirect   , typeof(JMP_Indirect   ) },
                new object[]{ Cpu.Instructions.JSR_Absolute   , typeof(JSR_Absolute   ) },
            };

        [Fact]
        public void Should_have_151_instructions()
        {
            var sb = new StringBuilder();
            var valid = 0;

            for (var i = 0; i < 256; i++)
            {
                Cpu.Reset();

                Memory[0x0000] = (byte)i;

                Cpu.Execute(1);
                if(Cpu.State.Instruction is InvalidOperation)
                {
                    sb.AppendLine($"0x{i:x}");
                }
                else
                {
                    valid++;
                }
            }

            valid.Should().Be(151);
        }
    }

    public class FetchMemory : CpuTests
    {
        [Fact]
        public void Should_return_value_at_given_address()
        {
            Memory[0x11] = 0x12;

            var result = Cpu.FetchMemory(0x11);

            result.Should().Be(Memory[0x11]);
        }

        [Fact]
        public void Should_return_value_at_address_defined_by_pc_when_given_address_is_null()
        {
            Memory[0x13] = 0x12;

            Cpu.Registers.PC = 0x13;
            var result = Cpu.FetchMemory();

            result.Should().Be(Memory[0x13]);
        }

        [Fact]
        public void Should_increase_state_ticks_by_one()
        {
            Cpu.FetchMemory();

            Cpu.State.Ticks.Should().Be(1);
        }

        [Fact]
        public void Should_increment_pc_when_called_without_address()
        {
            Memory[0x13] = 0x12;
            Memory[0x14] = 0x13;

            Cpu.Registers.PC = 0x13;

            Cpu.FetchMemory().Should().Be(Memory[0x13]);
            Cpu.FetchMemory().Should().Be(Memory[0x14]);
        }

        [Fact]
        public void Should_not_increment_pc_when_called_with_address()
        {
            Memory[0x13] = 0x12;
            Memory[0x14] = 0x13;

            Cpu.Registers.PC = 0x13;

            Cpu.FetchMemory(0x13).Should().Be(Memory[0x13]);
            Cpu.FetchMemory().Should().Be(Memory[0x13]);
        }

        [Fact]
        public void Should_reflect_address_and_data_read_on_addressbus_and_databus_with_RW_true()
        {
            ushort addr = 0x1234;
            byte data = 0x69;
            Memory[addr] = data;
            
            Cpu.FetchMemory(addr);

            Cpu.Pins.GetAddr().Should().Be(addr);
            Cpu.Pins.GetData().Should().Be(data);
            Cpu.Pins.RW.Should().Be(true);
        }
    }

    public class Pins : CpuTests
    {
        [Theory]
        [InlineData(0x0000)]
        [InlineData(0x5555)]
        [InlineData(0xAAAA)]
        [InlineData(0xFFFF)]
        public void SetAddr_should_set_A15_through_A0_to_match(ushort addr)
        {
            Cpu.Pins.SetAddr(addr);

            Cpu.Pins.A0.Should(). Be((addr & 0x0001) > 0);
            Cpu.Pins.A1.Should(). Be((addr & 0x0002) > 0);
            Cpu.Pins.A2.Should(). Be((addr & 0x0004) > 0);
            Cpu.Pins.A3.Should(). Be((addr & 0x0008) > 0);
            Cpu.Pins.A4.Should(). Be((addr & 0x0010) > 0);
            Cpu.Pins.A5.Should(). Be((addr & 0x0020) > 0);
            Cpu.Pins.A6.Should(). Be((addr & 0x0040) > 0);
            Cpu.Pins.A7.Should(). Be((addr & 0x0080) > 0);
            Cpu.Pins.A8.Should(). Be((addr & 0x0100) > 0);
            Cpu.Pins.A9.Should(). Be((addr & 0x0200) > 0);
            Cpu.Pins.A10.Should().Be((addr & 0x0400) > 0);
            Cpu.Pins.A11.Should().Be((addr & 0x0800) > 0);
            Cpu.Pins.A12.Should().Be((addr & 0x1000) > 0);
            Cpu.Pins.A13.Should().Be((addr & 0x2000) > 0);
            Cpu.Pins.A14.Should().Be((addr & 0x4000) > 0);
            Cpu.Pins.A15.Should().Be((addr & 0x8000) > 0);
        }

        [Theory]
        [InlineData(0x0000)]
        [InlineData(0x5555)]
        [InlineData(0xAAAA)]
        [InlineData(0xFFFF)]
        public void GetAddr_should_return_address_at_A15_through_A0(ushort addr)
        {
            Cpu.Pins.A0  = (addr & 0x0001) > 0;
            Cpu.Pins.A1  = (addr & 0x0002) > 0;
            Cpu.Pins.A2  = (addr & 0x0004) > 0;
            Cpu.Pins.A3  = (addr & 0x0008) > 0;
            Cpu.Pins.A4  = (addr & 0x0010) > 0;
            Cpu.Pins.A5  = (addr & 0x0020) > 0;
            Cpu.Pins.A6  = (addr & 0x0040) > 0;
            Cpu.Pins.A7  = (addr & 0x0080) > 0;
            Cpu.Pins.A8  = (addr & 0x0100) > 0;
            Cpu.Pins.A9  = (addr & 0x0200) > 0;
            Cpu.Pins.A10 = (addr & 0x0400) > 0;
            Cpu.Pins.A11 = (addr & 0x0800) > 0;
            Cpu.Pins.A12 = (addr & 0x1000) > 0;
            Cpu.Pins.A13 = (addr & 0x2000) > 0;
            Cpu.Pins.A14 = (addr & 0x4000) > 0;
            Cpu.Pins.A15 = (addr & 0x8000) > 0;

            Cpu.Pins.GetAddr().Should().Be(addr);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(0x55)]
        [InlineData(0xAA)]
        [InlineData(0xFF)]
        public void SetData_should_set_D7_through_D0_to_match(byte data)
        {
            Cpu.Pins.SetData(data);

            Cpu.Pins.D0.Should().Be((data & 0x0001) > 0);
            Cpu.Pins.D1.Should().Be((data & 0x0002) > 0);
            Cpu.Pins.D2.Should().Be((data & 0x0004) > 0);
            Cpu.Pins.D3.Should().Be((data & 0x0008) > 0);
            Cpu.Pins.D4.Should().Be((data & 0x0010) > 0);
            Cpu.Pins.D5.Should().Be((data & 0x0020) > 0);
            Cpu.Pins.D6.Should().Be((data & 0x0040) > 0);
            Cpu.Pins.D7.Should().Be((data & 0x0080) > 0);
        }

        [Theory]
        [InlineData(0x00)]
        [InlineData(0x55)]
        [InlineData(0xAA)]
        [InlineData(0xFF)]
        public void GetData_should_return_data_at_D7_through_D0(byte data)
        {
            Cpu.Pins.D0 = (data & 0x01) > 0;
            Cpu.Pins.D1 = (data & 0x02) > 0;
            Cpu.Pins.D2 = (data & 0x04) > 0;
            Cpu.Pins.D3 = (data & 0x08) > 0;
            Cpu.Pins.D4 = (data & 0x10) > 0;
            Cpu.Pins.D5 = (data & 0x20) > 0;
            Cpu.Pins.D6 = (data & 0x40) > 0;
            Cpu.Pins.D7 = (data & 0x80) > 0;

            Cpu.Pins.GetData().Should().Be(data);
        }
    }

    public class WriteMemory : CpuTests
    {
        [Fact]
        public void Should_set_value_at_given_address()
        {
            Cpu.WriteMemory(0x12, 0x5678);

            Memory[0x5678].Should().Be(0x12);
        }

        [Fact]
        public void Should_set_value_at_address_defined_by_pc_when_given_address_is_null()
        {
            Cpu.WriteMemory(0x15, 0x0005);

            Memory[0x0005].Should().Be(0x15);
        }

        [Fact]
        public void Should_increase_state_ticks_by_one()
        {
            Cpu.WriteMemory(0x00, 0x0000);

            Cpu.State.Ticks.Should().Be(1);
        }

        [Fact]
        public void Should_increment_pc_when_called_without_address()
        {
            Cpu.Registers.PC = 0x13;

            Cpu.WriteMemory(0x00);

            Cpu.Registers.PC.Should().Be(0x14);
        }

        [Fact]
        public void Should_not_increment_pc_when_called_with_address()
        {
            Cpu.Registers.PC = 0x15;

            Cpu.WriteMemory(0x00, 0x0000);

            Cpu.Registers.PC.Should().Be(0x15);
        }

        [Fact]
        public void Should_reflect_address_and_data_written_on_addressbus_and_databus_with_RW_false()
        {
            ushort addr = 0x4321;
            byte data = 0x69;
            Memory[addr] = data;

            Cpu.WriteMemory(data, addr);

            Cpu.Pins.GetAddr().Should().Be(addr);
            Cpu.Pins.GetData().Should().Be(data);
            Cpu.Pins.RW.Should().Be(false);
        }
    }

    public class SetRegister : CpuTests
    {
        [Theory]                      //NV BDIZC
        [InlineData(Register.A, 0x00, 0b00110110)]
        [InlineData(Register.A, 0x11, 0b00110100)]
        [InlineData(Register.A, 0xFF, 0b10110100)]

        [InlineData(Register.X, 0x00, 0b00110110)]
        [InlineData(Register.X, 0x11, 0b00110100)]
        [InlineData(Register.X, 0xFF, 0b10110100)]

        [InlineData(Register.Y, 0x00, 0b00110110)]
        [InlineData(Register.Y, 0x11, 0b00110100)]
        [InlineData(Register.Y, 0xFF, 0b10110100)]

        [InlineData(Register.SP, 0x00, 0b00110110)]
        [InlineData(Register.SP, 0x11, 0b00110100)]
        [InlineData(Register.SP, 0xFF, 0b10110100)]
        public void Should_set_selected_register_to_given_value_and_update_N_and_Z_flags(
            Register register,
            byte value,
            byte expected_flags)
        {
            Cpu.Reset();
            Cpu.SetRegister(register, value);

            Cpu.Flags.GetSR().Should().Be(expected_flags);

            switch (register)
            {
                case Register.A:
                    Cpu.Registers.A.Should().Be(value);
                    break;
                case Register.X:
                    Cpu.Registers.X.Should().Be(value);
                    break;
                case Register.Y:
                    Cpu.Registers.Y.Should().Be(value);
                    break;
                case Register.SP:
                    Cpu.Registers.SP.Should().Be(value);
                    break;
            }
        }
    }
}
