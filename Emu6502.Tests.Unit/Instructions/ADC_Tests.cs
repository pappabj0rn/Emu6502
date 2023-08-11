using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class ADC_Tests : InstructionTestBase
{
    protected abstract void ADC_instruction_test_memory_setup(ICpu cpu, byte expectedValue);

    [Theory]
    [InlineData(0x00, 0x01, false, 0x01, false, false, false)]
    [InlineData(0x01, 0x01, false, 0x02, false, false, false)]
    [InlineData(0x00, 0xFF, false, 0xFF, false, true, false)]
    [InlineData(0x01, 0xFF, false, 0x00, true, false, true)]
    [InlineData(0x7F, 0x01, false, 0x80, false, true, false)]
    [InlineData(0x80, 0x01, false, 0x81, false, true, false)]
    [InlineData(0xFF, 0x02, false, 0x01, false, false, true)]
    [InlineData(0x00, 0x01, true, 0x02, false, false, false)]
    [InlineData(0x01, 0x01, true, 0x03, false, false, false)]
    [InlineData(0x00, 0xFF, true, 0x00, true, false, true)]
    [InlineData(0x01, 0xFF, true, 0x01, false, false, true)]
    [InlineData(0x7F, 0x01, true, 0x81, false, true, false)]
    [InlineData(0x80, 0x01, true, 0x82, false, true, false)]
    [InlineData(0xFF, 0x02, true, 0x02, false, false, true)]
    public void Should_update_Z_and_N_and_C_flags_matching_result_stored_in_accumulator(
        byte inital_a,
        byte memory,
        bool initial_c,
        byte expected_a,
        bool expected_z,
        bool expected_n,
        bool expected_c)
    {
        ADC_instruction_test_memory_setup(CpuMock, memory);

        CpuMock.Registers.A = inital_a;
        CpuMock.Flags.C = initial_c;

        Sut.Execute(CpuMock);

        CpuMock.Registers.A.Should().Be(expected_a);
        CpuMock.Flags.Z.Should().Be(expected_z);
        CpuMock.Flags.N.Should().Be(expected_n);
        CpuMock.Flags.C.Should().Be(expected_c);
    }

    public class Immediate : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new ADC_Immediate();

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = expectedValue;
        }
    }

    public class Absolute : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ADC_Absolute();

        [Fact]
        public void Should_load_byte_at_address_following_instruction_into_accumulator()
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0x45;

            Memory[0x4523] = 0x01;

            CpuMock.Flags.C = true;
            CpuMock.Registers.A = 0x01;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x03);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = 0x12;
            Memory[0x0001] = 0x34;

            Memory[0x3412] = expectedValue;
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0x45;

            Memory[0x4523] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x55);
        }
    }

    public class AbsoluteX : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ADC_AbsoluteX();

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
            CpuMock.Registers.A.Should().Be(6);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0408] = value;

            CpuMock.Registers.X = 0x05;
        }

        [Fact]
        public void Should_require_4_cycles_when_x_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 4;

            Memory[0x0000] = 0x01;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0500] = 0x69;

            CpuMock.Registers.X = 0xFF;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x69);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }
    
    public class AbsoluteY : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ADC_AbsoluteY();

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
            CpuMock.Registers.A.Should().Be(6);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte value)
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

            CpuMock.Registers.A.Should().Be(0x69);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }

    public class Zeropage : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new ADC_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0020] = 0x01;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(1);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0xff;

            Memory[0x0003] = value;
        }
    }

    public class ZeropageX : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ADC_ZeropageX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0031] = 0x01;

            CpuMock.Registers.X = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(1);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte value)
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

            CpuMock.Registers.A.Should().Be(0x65);
        }
    }

    public class IndirectX : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 5;
        protected override Instruction Sut { get; } = new ADC_IndirectX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0031] = 0x01;
            Memory[0x0032] = 0x02;

            Memory[0x0201] = 0x05;

            CpuMock.Registers.X = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x05);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0034] = 0x01;
            Memory[0x0035] = 0x01;

            Memory[0x0101] = value;

            CpuMock.Registers.X = 0x11;
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary_when_fetching_indirect_address()
        {
            Memory[0x0000] = 0xff;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0x01;

            Memory[0x0101] = 0x74;

            CpuMock.Registers.X = 0x02;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x74);
        }
    }

    public class IndirectY : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 4;
        protected override Instruction Sut { get; } = new ADC_IndirectY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x70;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0070] = 0x43;
            Memory[0x0071] = 0x35;

            Memory[0x3553] = 0x23;

            CpuMock.Registers.Y = 0x10;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x23);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0023] = 0x01;
            Memory[0x0024] = 0x02;

            Memory[0x0212] = value;

            CpuMock.Registers.Y = 0x11;
        }

        [Fact]
        public void Should_require_5_cycles_when_y_indexing_causes_page_transition()
        {
            Memory[0x0000] = 0x70;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0070] = 0x43;
            Memory[0x0071] = 0x35;

            Memory[0x3642] = 0x78;

            CpuMock.Registers.Y = 0xFF;

            CpuMock.State.RemainingCycles = 5;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x78);
            CpuMock.State.Ticks.Should().Be(5);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }
}
