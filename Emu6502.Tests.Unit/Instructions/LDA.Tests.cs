using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class LDA : InstructionTestBase
{
    public class Immediate : LDA
    {
        protected Instruction _sut = new LDA_Immediate();

        public Immediate()
        {
            State.Instruction = _sut;
        }

        [Fact]
        public void Should_execute_in_one_cycle()
        {
            State.RemainingCycles = 1;

            _sut.Execute(CpuMock);

            State.RemainingCycles.Should().Be(0);
            State.Ticks.Should().Be(1);
            State.Instruction.Should().BeNull();
        }

        [Fact]
        public void Should_load_byte_following_instruction_into_accumulator()
        {
            CpuMock
            .FetchMemory()
            .Returns((byte)0x01);

            _sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x01);
        }

        [Theory]
        [InlineData(0x00, true, false)]
        [InlineData(0x01, false, false)]
        [InlineData(0x7F, false, false)]
        [InlineData(0x80, false, true)]
        [InlineData(0x81, false, true)]
        [InlineData(0xFF, false, true)]
        public void Should_update_Z_and_N_flags_matching_value_loaded_into_accumulator(byte value, bool expected_z, bool expected_n)
        {
            CpuMock
                .FetchMemory()
                .Returns(value);

            _sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(value);
            CpuMock.Flags.Z.Should().Be(expected_z);
            CpuMock.Flags.N.Should().Be(expected_n);
        }
    }
}
