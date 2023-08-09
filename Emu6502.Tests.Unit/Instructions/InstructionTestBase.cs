using Emu6502.Instructions;
namespace Emu6502.Tests.Unit.Instructions;

public abstract class InstructionTestBase
{
    protected ICpu CpuMock = Substitute.For<ICpu>();
    protected ExecutionState State = new();

    protected abstract Instruction Sut { get; }
    public abstract int NumberOfCyclesForExecution { get; }
    

    public InstructionTestBase()
    {
        CpuMock.State.Returns(State);

        CpuMock
            .FetchMemory()
            .ReturnsForAnyArgs((byte)0x00)
            .AndDoes(x =>State.Tick());
    }

    [Fact]
    public void Should_execute_in_defined_number_of_cycles()
    {
        State.RemainingCycles = NumberOfCyclesForExecution;

        Sut.Execute(CpuMock);

        State.RemainingCycles.Should().Be(0);
        State.Ticks.Should().Be(NumberOfCyclesForExecution);
        State.Instruction.Should().BeNull();
    }
}