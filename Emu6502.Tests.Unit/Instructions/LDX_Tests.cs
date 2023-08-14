using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class LDX_Tests : InstructionTestBase
{
    protected abstract void LDX_instruction_test_memory_setup(ICpu cpu, byte expectedValue);

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
        LDX_instruction_test_memory_setup(CpuMock, value);

        Sut.Execute(CpuMock);

        CpuMock.Registers.X.Should().Be(value);
        CpuMock.Flags.Z.Should().Be(expected_z);
        CpuMock.Flags.N.Should().Be(expected_n);
    }

    public class Immediate : LDX_Tests
    {
        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new LDX_Immediate();

        protected override void LDX_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = expectedValue;
        }
    }

    public class Absolute : LDX_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDX_Absolute();

        [Fact]
        public void Should_load_byte_at_address_following_instruction_into_accumulator()
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x00;
            Memory[0x0002] = 0x00;
            Memory[0x0003] = 0x01;

            Sut.Execute(CpuMock);

            CpuMock.Registers.X.Should().Be(0x01);
        }

        protected override void LDX_instruction_test_memory_setup(ICpu cpu, byte value)
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
            CpuMock.Registers.X.Should().Be(1);
        }
    }
    public class AbsoluteY : LDX_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDX_AbsoluteY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xff;

            Memory[0x0105] = 0x06;

            CpuMock.Registers.Y = 0x03;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.X.Should().Be(6);
        }

        protected override void LDX_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0408] = value;

            CpuMock.Registers.Y = 0x05;
        }

        [Fact]
        public void Should_require_4_cycles_when_y_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 4;

            Memory[0x0000] = 0x01;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0500] = 0x69;

            CpuMock.Registers.Y = 0xFF;

            Sut.Execute(CpuMock);

            CpuMock.Registers.X.Should().Be(0x69);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }
    public class Zeropage : LDX_Tests
    {
        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new LDX_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0020] = 0x01;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.X.Should().Be(1);
        }

        protected override void LDX_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0xff;

            Memory[0x0003] = value;
        }
    }

    public class ZeropageY : LDX_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDX_ZeropageY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0031] = 0x01;

            CpuMock.Registers.Y = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.X.Should().Be(1);
        }

        protected override void LDX_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;

            Memory[0x0034] = value;

            CpuMock.Registers.Y = 0x11;
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary()
        {
            Memory[0x0000] = 0xff;
            Memory[0x0001] = 0x65;
            Memory[0x0002] = 0xff;

            CpuMock.Registers.Y = 0x02;

            Sut.Execute(CpuMock);

            CpuMock.Registers.X.Should().Be(0x65);
        }
    }
}
