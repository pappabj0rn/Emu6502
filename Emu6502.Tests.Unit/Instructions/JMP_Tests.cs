using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class JMP_Tests : InstructionTestBase
{
    public class Absolute : JMP_Tests
    {

        public Absolute()
        {
            State.Instruction = Sut;
        }

        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new JMP_Absolute();

        [Fact]
        public void Should_set_pc_to_address_following_instruction()
        {
            CpuMock
                .FetchMemory()
                .Returns(
                    (byte)0x11, 
                    (byte)0x22
                );

            Sut.Execute(CpuMock);

            CpuMock.Registers.PC.Should().Be(0x2211);
        }
    }    
}
