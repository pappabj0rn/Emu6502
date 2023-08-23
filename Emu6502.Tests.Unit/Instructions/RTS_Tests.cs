namespace Emu6502.Tests.Unit.Instructions;

public class RTS_Tests : InstructionTestBase
{
    public RTS_Tests(ITestOutputHelper output) : base(output) { }

    public override int NumberOfCyclesForExecution => 5;

    protected override Instruction Sut { get; } = new RTS();

    public override void SteppedThroughSetup()
    {
        CpuMock.Registers.SP = 0xFD;
        Memory[0x01FE] = 0x01;
        Memory[0x01FF] = 0x00;
    }

    public override void SteppedThroughVerification()
    {
        CpuMock.Registers.PC.Should().Be(0x0002);
        CpuMock.Registers.SP.Should().Be(0xFF);
    }
}
