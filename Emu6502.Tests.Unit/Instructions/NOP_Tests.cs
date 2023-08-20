using Emu6502.Instructions;
using Xunit.Abstractions;

namespace Emu6502.Tests.Unit.Instructions;

public class NOP_Tests : InstructionTestBase
{
    public NOP_Tests(ITestOutputHelper output) : base(output)
    {
        State.Instruction = Sut;
    }

    public override int NumberOfCyclesForExecution => 1;
    protected override Instruction Sut { get; } = new NOP();
}
