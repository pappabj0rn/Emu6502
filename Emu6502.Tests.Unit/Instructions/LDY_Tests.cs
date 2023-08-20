﻿using Emu6502.Instructions;
using Xunit.Abstractions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class LDY_Tests : InstructionTestBase
{
    public LDY_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void LDY_instruction_test_memory_setup(ICpu cpu, byte expectedValue);

    [Theory]
    [InlineData(0x00, true, false)]
    [InlineData(0x01, false, false)]
    [InlineData(0x7F, false, false)]
    [InlineData(0x80, false, true)]
    [InlineData(0x81, false, true)]
    [InlineData(0xFF, false, true)]
    public void Should_update_Z_and_N_flags_matching_value_loaded_into_accumulator(
            byte value,
            bool expected_z,
            bool expected_n)
    {
        LDY_instruction_test_memory_setup(CpuMock, value);

        Sut.Execute(CpuMock);

        CpuMock.Registers.Y.Should().Be(value);
        CpuMock.Flags.Z.Should().Be(expected_z);
        CpuMock.Flags.N.Should().Be(expected_n);
    }

    public class Immediate : LDY_Tests
    {
        public Immediate(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new LDY_Immediate();

        protected override void LDY_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = expectedValue;
        }
    }

    public class Absolute : LDY_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDY_Absolute();

        [Fact]
        public void Should_load_byte_at_address_following_instruction_into_accumulator()
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x00;
            Memory[0x0002] = 0x00;
            Memory[0x0003] = 0x01;

            Sut.Execute(CpuMock);

            CpuMock.Registers.Y.Should().Be(0x01);
        }

        protected override void LDY_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0x00;

            Memory[0x0403] = value;
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xFF;

            Memory[0x0102] = 0x01;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.Y.Should().Be(1);
        }
    }

    public class AbsoluteX : LDY_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDY_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xff;

            Memory[0x0105] = 0x06;

            CpuMock.Registers.X = 0x03;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.Y.Should().Be(6);
        }

        protected override void LDY_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0408] = value;

            CpuMock.Registers.X = 0x05;
        }

        [Fact]
        public void Should_require_4_cycles_when_y_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 4;

            Memory[0x0000] = 0x01;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0500] = 0x69;

            CpuMock.Registers.X = 0xFF;

            Sut.Execute(CpuMock);

            CpuMock.Registers.Y.Should().Be(0x69);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }
    public class Zeropage : LDY_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new LDY_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0020] = 0x01;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.Y.Should().Be(1);
        }

        protected override void LDY_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0xff;

            Memory[0x0003] = value;
        }
    }

    public class ZeropageX : LDY_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDY_ZeropageX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0031] = 0x01;

            CpuMock.Registers.X = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.Y.Should().Be(1);
        }

        protected override void LDY_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;

            Memory[0x0034] = value;

            CpuMock.Registers.X = 0x11;
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary()
        {
            Memory[0x0000] = 0xff;
            Memory[0x0001] = 0x65;
            Memory[0x0002] = 0xff;

            CpuMock.Registers.X = 0x02;

            Sut.Execute(CpuMock);

            CpuMock.Registers.Y.Should().Be(0x65);
        }
    }
}
