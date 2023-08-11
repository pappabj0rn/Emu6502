﻿using Emu6502.Instructions;

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
        [InlineData(Cpu.Instructions.LDA_PostIndexedIndirectZeropageY, typeof(LDA_PostIndexedIndirectZeropageY))]
        [InlineData(Cpu.Instructions.NOP, typeof(NOP))]
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
