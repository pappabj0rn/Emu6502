using Emu6502.Instructions;

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
            Cpu.Flags.I.Should().BeFalse();
            Cpu.Flags.Z.Should().BeFalse();
            Cpu.Flags.C.Should().BeFalse();

            Cpu.Registers.A.Should().Be(0x00);
            Cpu.Registers.X.Should().Be(0x00);
            Cpu.Registers.Y.Should().Be(0x00);
            Cpu.Registers.S.Should().Be(0x00);
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

        [InlineData(Cpu.Instructions.NOP, typeof(NOP))]

        [InlineData(Cpu.Instructions.CLC, typeof(CLC))]
        [InlineData(Cpu.Instructions.CLD, typeof(CLD))]
        [InlineData(Cpu.Instructions.CLI, typeof(CLI))]
        [InlineData(Cpu.Instructions.CLV, typeof(CLV))]

        [InlineData(Cpu.Instructions.JMP_Absolute, typeof(JMP_Absolute))]
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
}
