using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class JMP_Tests : InstructionTestBase
{
    public class Absolute : JMP_Tests
    {
        protected Instruction _sut = new JMP_Absolute();

        public Absolute()
        {
            State.Instruction = _sut;
        }

        [Fact]
        public void Should_execute_in_two_cycle()
        {
            State.RemainingCycles = 2;

            _sut.Execute(CpuMock);

            State.RemainingCycles.Should().Be(0);
            State.Ticks.Should().Be(2);
            State.Instruction.Should().BeNull();
        }

        [Fact]
        public void Should_set_pc_to_address_following_instruction()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x11, 
                    (byte)0x22
                );

            _sut.Execute(CpuMock);

            CpuMock.Registers.PC.Should().Be(0x2211);
        }
    }    
}
