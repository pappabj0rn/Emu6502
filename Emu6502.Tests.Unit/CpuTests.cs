namespace Emu6502.Tests.Unit;

public abstract class CpuTests
{
    private Cpu Cpu;
    private byte[] Memory;

    public CpuTests()
    {
        Memory = new byte[0xffff];
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
            Memory[0x00] = Cpu.Instructions.Test_2cycle;

            //fetch
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(1);
            Cpu.Registers.PC.Should().Be(0x01);

            //cycle 1
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(2);
            Cpu.Registers.X.Should().Be(1);
            Cpu.Registers.PC.Should().Be(0x01);

            //cycle 2
            Cpu.Execute(1);
            Cpu.Ticks.Should().Be(3);
            Cpu.Registers.X.Should().Be(2);
            Cpu.Registers.PC.Should().Be(0x01);
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
        [InlineData(Cpu.Instructions.LDA_Immediate, typeof(LDA_Immediate))]
        [InlineData(Cpu.Instructions.LDA_Absolute, typeof(LDA_Absolute))]
        [InlineData(Cpu.Instructions.LDA_AbsoluteX, typeof(LDA_AbsoluteX))]
        [InlineData(Cpu.Instructions.LDA_AbsoluteY, typeof(LDA_AbsoluteY))]
        [InlineData(Cpu.Instructions.LDA_Zeropage, typeof(LDA_Zeropage))]
        [InlineData(Cpu.Instructions.LDA_ZeropageX, typeof(LDA_ZeropageX))]
        [InlineData(Cpu.Instructions.LDA_IndirectX, typeof(LDA_IndirectX))]
        [InlineData(Cpu.Instructions.LDA_IndirectY, typeof(LDA_IndirectY))]

        [InlineData(Cpu.Instructions.LDX_Immediate, typeof(LDX_Immediate))]
        [InlineData(Cpu.Instructions.LDX_Absolute, typeof(LDX_Absolute))]
        [InlineData(Cpu.Instructions.LDX_AbsoluteY, typeof(LDX_AbsoluteY))]
        [InlineData(Cpu.Instructions.LDX_Zeropage, typeof(LDX_Zeropage))]
        [InlineData(Cpu.Instructions.LDX_ZeropageY, typeof(LDX_ZeropageY))]

        [InlineData(Cpu.Instructions.LDY_Immediate, typeof(LDY_Immediate))]
        [InlineData(Cpu.Instructions.LDY_Absolute, typeof(LDY_Absolute))]
        [InlineData(Cpu.Instructions.LDY_AbsoluteX, typeof(LDY_AbsoluteX))]
        [InlineData(Cpu.Instructions.LDY_Zeropage, typeof(LDY_Zeropage))]
        [InlineData(Cpu.Instructions.LDY_ZeropageX, typeof(LDY_ZeropageX))]

        [InlineData(Cpu.Instructions.ADC_Immediate, typeof(ADC_Immediate))]
        [InlineData(Cpu.Instructions.ADC_Absolute, typeof(ADC_Absolute))]
        [InlineData(Cpu.Instructions.ADC_AbsoluteX, typeof(ADC_AbsoluteX))]
        [InlineData(Cpu.Instructions.ADC_AbsoluteY, typeof(ADC_AbsoluteY))]
        [InlineData(Cpu.Instructions.ADC_Zeropage, typeof(ADC_Zeropage))]
        [InlineData(Cpu.Instructions.ADC_ZeropageX, typeof(ADC_ZeropageX))]
        [InlineData(Cpu.Instructions.ADC_IndirectX, typeof(ADC_IndirectX))]
        [InlineData(Cpu.Instructions.ADC_IndirectY, typeof(ADC_IndirectY))]

        [InlineData(Cpu.Instructions.SBC_Immediate, typeof(SBC_Immediate))]
        [InlineData(Cpu.Instructions.SBC_Absolute, typeof(SBC_Absolute))]
        [InlineData(Cpu.Instructions.SBC_AbsoluteX, typeof(SBC_AbsoluteX))]
        [InlineData(Cpu.Instructions.SBC_AbsoluteY, typeof(SBC_AbsoluteY))]
        [InlineData(Cpu.Instructions.SBC_Zeropage, typeof(SBC_Zeropage))]
        [InlineData(Cpu.Instructions.SBC_ZeropageX, typeof(SBC_ZeropageX))]
        [InlineData(Cpu.Instructions.SBC_IndirectX, typeof(SBC_IndirectX))]
        [InlineData(Cpu.Instructions.SBC_IndirectY, typeof(SBC_IndirectY))]

        [InlineData(Cpu.Instructions.ORA_Immediate, typeof(ORA_Immediate))]
        [InlineData(Cpu.Instructions.ORA_Absolute, typeof(ORA_Absolute))]
        [InlineData(Cpu.Instructions.ORA_AbsoluteX, typeof(ORA_AbsoluteX))]
        [InlineData(Cpu.Instructions.ORA_AbsoluteY, typeof(ORA_AbsoluteY))]
        [InlineData(Cpu.Instructions.ORA_Zeropage, typeof(ORA_Zeropage))]
        [InlineData(Cpu.Instructions.ORA_ZeropageX, typeof(ORA_ZeropageX))]
        [InlineData(Cpu.Instructions.ORA_IndirectX, typeof(ORA_IndirectX))]
        [InlineData(Cpu.Instructions.ORA_IndirectY, typeof(ORA_IndirectY))]

        [InlineData(Cpu.Instructions.EOR_Immediate, typeof(EOR_Immediate))]
        [InlineData(Cpu.Instructions.EOR_Absolute, typeof(EOR_Absolute))]
        [InlineData(Cpu.Instructions.EOR_AbsoluteX, typeof(EOR_AbsoluteX))]
        [InlineData(Cpu.Instructions.EOR_AbsoluteY, typeof(EOR_AbsoluteY))]
        [InlineData(Cpu.Instructions.EOR_Zeropage, typeof(EOR_Zeropage))]
        [InlineData(Cpu.Instructions.EOR_ZeropageX, typeof(EOR_ZeropageX))]
        [InlineData(Cpu.Instructions.EOR_IndirectX, typeof(EOR_IndirectX))]
        [InlineData(Cpu.Instructions.EOR_IndirectY, typeof(EOR_IndirectY))]

        [InlineData(Cpu.Instructions.AND_Immediate, typeof(AND_Immediate))]
        [InlineData(Cpu.Instructions.AND_Absolute, typeof(AND_Absolute))]
        [InlineData(Cpu.Instructions.AND_AbsoluteX, typeof(AND_AbsoluteX))]
        [InlineData(Cpu.Instructions.AND_AbsoluteY, typeof(AND_AbsoluteY))]
        [InlineData(Cpu.Instructions.AND_Zeropage, typeof(AND_Zeropage))]
        [InlineData(Cpu.Instructions.AND_ZeropageX, typeof(AND_ZeropageX))]
        [InlineData(Cpu.Instructions.AND_IndirectX, typeof(AND_IndirectX))]
        [InlineData(Cpu.Instructions.AND_IndirectY, typeof(AND_IndirectY))]

        [InlineData(Cpu.Instructions.DEC_Zeropage, typeof(DEC_Zeropage))]
        [InlineData(Cpu.Instructions.DEC_ZeropageX, typeof(DEC_ZeropageX))]
        [InlineData(Cpu.Instructions.DEC_Absolute, typeof(DEC_Absolute))]
        [InlineData(Cpu.Instructions.DEC_AbsoluteX, typeof(DEC_AbsoluteX))]
        [InlineData(Cpu.Instructions.DEX, typeof(DEX))]
        [InlineData(Cpu.Instructions.DEY, typeof(DEY))]

        [InlineData(Cpu.Instructions.INC_Zeropage, typeof(INC_Zeropage))]
        [InlineData(Cpu.Instructions.INC_ZeropageX, typeof(INC_ZeropageX))]
        [InlineData(Cpu.Instructions.INC_Absolute, typeof(INC_Absolute))]
        [InlineData(Cpu.Instructions.INC_AbsoluteX, typeof(INC_AbsoluteX))]
        [InlineData(Cpu.Instructions.INX, typeof(INX))]
        [InlineData(Cpu.Instructions.INY, typeof(INY))]

        [InlineData(Cpu.Instructions.NOP, typeof(NOP))]

        [InlineData(Cpu.Instructions.CLC, typeof(CLC))]
        [InlineData(Cpu.Instructions.CLD, typeof(CLD))]
        [InlineData(Cpu.Instructions.CLI, typeof(CLI))]
        [InlineData(Cpu.Instructions.CLV, typeof(CLV))]

        [InlineData(Cpu.Instructions.SEC, typeof(SEC))]
        [InlineData(Cpu.Instructions.SED, typeof(SED))]
        [InlineData(Cpu.Instructions.SEI, typeof(SEI))]

        [InlineData(Cpu.Instructions.PHA, typeof(PHA))]
        [InlineData(Cpu.Instructions.PLA, typeof(PLA))]
        [InlineData(Cpu.Instructions.PHP, typeof(PHP))]
        [InlineData(Cpu.Instructions.PLP, typeof(PLP))]

        [InlineData(Cpu.Instructions.TAX, typeof(TAX))]
        [InlineData(Cpu.Instructions.TAY, typeof(TAY))]
        [InlineData(Cpu.Instructions.TXA, typeof(TXA))]
        [InlineData(Cpu.Instructions.TXS, typeof(TXS))]
        [InlineData(Cpu.Instructions.TSX, typeof(TSX))]
        [InlineData(Cpu.Instructions.TYA, typeof(TYA))]

        [InlineData(Cpu.Instructions.STA_Zeropage, typeof(STA_Zeropage))]
        [InlineData(Cpu.Instructions.STA_ZeropageX, typeof(STA_ZeropageX))]
        [InlineData(Cpu.Instructions.STA_Absolute, typeof(STA_Absolute))]
        [InlineData(Cpu.Instructions.STA_AbsoluteX, typeof(STA_AbsoluteX))]
        [InlineData(Cpu.Instructions.STA_AbsoluteY, typeof(STA_AbsoluteY))]
        [InlineData(Cpu.Instructions.STA_IndirectX, typeof(STA_IndirectX))]
        [InlineData(Cpu.Instructions.STA_IndirectY, typeof(STA_IndirectY))]

        [InlineData(Cpu.Instructions.STX_Zeropage, typeof(STX_Zeropage))]
        [InlineData(Cpu.Instructions.STX_ZeropageY, typeof(STX_ZeropageY))]
        [InlineData(Cpu.Instructions.STX_Absolute, typeof(STX_Absolute))]

        [InlineData(Cpu.Instructions.STY_Zeropage, typeof(STY_Zeropage))]
        [InlineData(Cpu.Instructions.STY_ZeropageX, typeof(STY_ZeropageX))]
        [InlineData(Cpu.Instructions.STY_Absolute, typeof(STY_Absolute))]

        [InlineData(Cpu.Instructions.ASL_Accumulator, typeof(ASL_Accumulator))]
        [InlineData(Cpu.Instructions.ASL_Zeropage, typeof(ASL_Zeropage))]
        [InlineData(Cpu.Instructions.ASL_ZeropageX, typeof(ASL_ZeropageX))]
        [InlineData(Cpu.Instructions.ASL_Absolute, typeof(ASL_Absolute))]
        [InlineData(Cpu.Instructions.ASL_AbsoluteX, typeof(ASL_AbsoluteX))]

        [InlineData(Cpu.Instructions.LSR_Accumulator, typeof(LSR_Accumulator))]
        [InlineData(Cpu.Instructions.LSR_Zeropage, typeof(LSR_Zeropage))]
        [InlineData(Cpu.Instructions.LSR_ZeropageX, typeof(LSR_ZeropageX))]
        [InlineData(Cpu.Instructions.LSR_Absolute, typeof(LSR_Absolute))]
        [InlineData(Cpu.Instructions.LSR_AbsoluteX, typeof(LSR_AbsoluteX))]

        [InlineData(Cpu.Instructions.ROL_Accumulator, typeof(ROL_Accumulator))]
        [InlineData(Cpu.Instructions.ROL_Zeropage, typeof(ROL_Zeropage))]
        [InlineData(Cpu.Instructions.ROL_ZeropageX, typeof(ROL_ZeropageX))]
        [InlineData(Cpu.Instructions.ROL_Absolute, typeof(ROL_Absolute))]
        [InlineData(Cpu.Instructions.ROL_AbsoluteX, typeof(ROL_AbsoluteX))]

        [InlineData(Cpu.Instructions.ROR_Accumulator, typeof(ROR_Accumulator))]
        [InlineData(Cpu.Instructions.ROR_Zeropage, typeof(ROR_Zeropage))]
        [InlineData(Cpu.Instructions.ROR_ZeropageX, typeof(ROR_ZeropageX))]
        [InlineData(Cpu.Instructions.ROR_Absolute, typeof(ROR_Absolute))]
        [InlineData(Cpu.Instructions.ROR_AbsoluteX, typeof(ROR_AbsoluteX))]

        [InlineData(Cpu.Instructions.CMP_Immediate, typeof(CMP_Immediate))]
        [InlineData(Cpu.Instructions.CMP_Zeropage, typeof(CMP_Zeropage))]
        [InlineData(Cpu.Instructions.CMP_ZeropageX, typeof(CMP_ZeropageX))]
        [InlineData(Cpu.Instructions.CMP_Absolute, typeof(CMP_Absolute))]
        [InlineData(Cpu.Instructions.CMP_AbsoluteX, typeof(CMP_AbsoluteX))]
        [InlineData(Cpu.Instructions.CMP_AbsoluteY, typeof(CMP_AbsoluteY))]
        [InlineData(Cpu.Instructions.CMP_IndirectX, typeof(CMP_IndirectX))]
        [InlineData(Cpu.Instructions.CMP_IndirectY, typeof(CMP_IndirectY))]

        [InlineData(Cpu.Instructions.CPX_Immediate, typeof(CPX_Immediate))]
        [InlineData(Cpu.Instructions.CPX_Zeropage, typeof(CPX_Zeropage))]
        [InlineData(Cpu.Instructions.CPX_Absolute, typeof(CPX_Absolute))]

        [InlineData(Cpu.Instructions.CPY_Immediate, typeof(CPY_Immediate))]
        [InlineData(Cpu.Instructions.CPY_Zeropage, typeof(CPY_Zeropage))]
        [InlineData(Cpu.Instructions.CPY_Absolute, typeof(CPY_Absolute))]

        [InlineData(Cpu.Instructions.JMP_Absolute, typeof(JMP_Absolute))]
        [InlineData(Cpu.Instructions.JMP_Indirect, typeof(JMP_Indirect))]

        [InlineData(Cpu.Instructions.JSR_Absolute, typeof(JSR_Absolute))]

        [InlineData(Cpu.Instructions.BIT_Zeropage, typeof(BIT_Zeropage))]
        [InlineData(Cpu.Instructions.BIT_Absolute, typeof(BIT_Absolute))]
        public void Should_not_throw_IOE_for_valid_instruction(byte instruction, Type expectedType)
        {
            Memory[0x00] = instruction;
            Cpu.Execute(1);

            Cpu.State.Instruction.Should().BeOfType(expectedType);
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
