using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class LDA_Tests : InstructionTestBase
{
    public class Immediate : LDA_Tests
    {
        public Immediate()
        {
            State.Instruction = Sut;
        }

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
            CpuMock
                .FetchMemory()
                .Returns(value);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(value);
            CpuMock.Flags.Z.Should().Be(expected_z);
            CpuMock.Flags.N.Should().Be(expected_n);
        }
    }

    public class Absolute : LDA_Tests
    {
        public Absolute()
        {
            State.Instruction = Sut;
        }

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

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(value);
            CpuMock.Flags.Z.Should().Be(expected_z);
            CpuMock.Flags.N.Should().Be(expected_n);
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
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x03,
                    (byte)0xFF
                );

            CpuMock
                .FetchMemory(0x0003)
                .Returns(value);

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(value);
            CpuMock.Flags.Z.Should().Be(expected_z);
            CpuMock.Flags.N.Should().Be(expected_n);
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

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(value);
            CpuMock.Flags.Z.Should().Be(expected_z);
            CpuMock.Flags.N.Should().Be(expected_n);
        }
    }
}
