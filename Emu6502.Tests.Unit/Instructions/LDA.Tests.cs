using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class LDA_Tests : InstructionTestBase
{
    protected abstract void LDA_instruction_test_memory_setup(ICpu cpu, byte expectedValue);

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
        LDA_instruction_test_memory_setup(CpuMock, value);
        
        Sut.Execute(CpuMock);

        CpuMock.Registers.A.Should().Be(value);
        CpuMock.Flags.Z.Should().Be(expected_z);
        CpuMock.Flags.N.Should().Be(expected_n);
    }

    public class Immediate : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new LDA_Immediate();

        [Fact]
        public void Should_load_byte_following_instruction_into_accumulator()
        {
            CpuMock
            .FetchMemory()
            .Returns((byte)0x01);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x01);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            CpuMock
                .FetchMemory()
                .Returns(expectedValue);
        }
    }

    public class Absolute : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDA_Absolute();

        [Fact]
        public void Should_load_byte_at_address_following_instruction_into_accumulator()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x03,
                    (byte)0x00,
                    (byte)0x00
                );

            CpuMock
                .FetchMemory(0x0003)
                .Returns((byte)0x01);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x01);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x03,
                    (byte)0x04,
                    (byte)0x00
                );

            CpuMock
                .FetchMemory(0x0403)
                .Returns(value);
        }

        public override void SteppedThroughSetup()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x02,
                    (byte)0x01,
                    (byte)0xFF
                );

            CpuMock
                .FetchMemory(0x0102)
                .Returns((byte)0x01);
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(1);
        }
    }

    public class AbsoluteX : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDA_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x02,
                    (byte)0x01,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x03;

            CpuMock
                .FetchMemory(0x0105)
                .Returns((byte)0x06);
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(6);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x03,
                    (byte)0x04,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x05;

            CpuMock
                .FetchMemory(0x0408)
                .Returns(value);
        }

        [Fact]
        public void Should_require_4_cycles_when_x_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 5;

            CpuMock
                .FetchMemory(Arg.Is<ushort?>(x => x == null))
                .Returns(
                    (byte)0x01,
                    (byte)0x04,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0xFF;

            CpuMock
                .FetchMemory(Arg.Is<ushort>(0x0500))
                .Returns((byte)0x69);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x69);
            CpuMock.State.Ticks.Should().Be(4);
        }
    }

    public class AbsoluteY : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDA_AbsoluteY();

        public override void SteppedThroughSetup()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x02,
                    (byte)0x01,
                    (byte)0xFF
                );

            CpuMock.Registers.Y = 0x03;

            CpuMock
                .FetchMemory(0x0105)
                .Returns((byte)0x06);
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(6);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x03,
                    (byte)0x04,
                    (byte)0xFF
                );

            CpuMock.Registers.Y = 0x05;

            CpuMock
                .FetchMemory(0x0408)
                .Returns(value);
        }

        [Fact]
        public void Should_require_4_cycles_when_y_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 5;

            CpuMock
                .FetchMemory(Arg.Is<ushort?>(x => x == null))
                .Returns(
                    (byte)0x01,
                    (byte)0x04,
                    (byte)0xFF
                );

            CpuMock.Registers.Y = 0xFF;

            CpuMock
                .FetchMemory(Arg.Is<ushort>(0x0500))
                .Returns((byte)0x69);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x69);
            CpuMock.State.Ticks.Should().Be(4);
        }
    }

    public class Zeropage : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new LDA_Zeropage();

        public override void SteppedThroughSetup()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x20,
                    (byte)0xFF
                );

            CpuMock
                .FetchMemory(0x0020)
                .Returns((byte)0x01);
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(1);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x03,
                    (byte)0xFF
                );

            CpuMock
                .FetchMemory(0x0003)
                .Returns(value);
        }
    }

    public class ZeropageX : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new LDA_ZeropageX();

        public override void SteppedThroughSetup()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x20,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x11;

            CpuMock
                .FetchMemory(0x0031)
                .Returns((byte)0x01);
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(1);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x23,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x11;

            CpuMock
                .FetchMemory(0x0034)
                .Returns(value);
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x02;

            CpuMock
                .FetchMemory(0x0001)
                .Returns((byte)0x65);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x65);
        }
    }

    public class PreIndexedIndirectZeropageX : LDA_Tests
    {
        public override int NumberOfCyclesForExecution => 5;
        protected override Instruction Sut { get; } = new LDA_PreIndexedIndirectZeropageX();

        public override void SteppedThroughSetup()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x20,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x11;

            CpuMock
                .FetchMemory(0x0031)
                .Returns((byte)0x01);

            CpuMock
                .FetchMemory(0x0032)
                .Returns((byte)0x02);

            CpuMock
                .FetchMemory(0x0201)
                .Returns((byte)0x05);
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x05);
        }

        protected override void LDA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x23,
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x11;

            CpuMock
                .FetchMemory(0x0034)
                .Returns((byte)0x01);

            CpuMock
                .FetchMemory(0x0035)
                .Returns((byte)0x01);

            CpuMock
                .FetchMemory(0x0101)
                .Returns(value);
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary_when_fetching_indirect_address()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0xFF
                );

            CpuMock.Registers.X = 0x02;

            CpuMock
                .FetchMemory(0x0001)
                .Returns((byte)0x01);

            CpuMock
                .FetchMemory(0x0002)
                .Returns((byte)0x01);

            CpuMock
                .FetchMemory(0x0101)
                .Returns((byte)0x74);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x74);
        }
    }
}
