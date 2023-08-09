using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public class NOP_Tests : InstructionTestBase
{
    protected Instruction _sut = new NOP();

    public NOP_Tests()
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
}
