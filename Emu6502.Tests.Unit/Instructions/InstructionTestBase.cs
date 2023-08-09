namespace Emu6502.Tests.Unit.Instructions;

public abstract class InstructionTestBase
{
    protected ICpu CpuMock = Substitute.For<ICpu>();
    protected ExecutionState State = new();
    

    public InstructionTestBase()
    {
        CpuMock.State.Returns(State);

        CpuMock
            .FetchMemory()
            .Returns((byte)0x00)
            .AndDoes(x =>State.Tick());
    }
}