namespace Emu6502.Tests.Unit.Instructions;

public class RTI_Tests : InstructionTestBase
{
    public RTI_Tests(ITestOutputHelper output) : base(output) { }

    public override int NumberOfCyclesForExecution => 5;

    protected override Instruction Sut { get; } = new RTI();

    public override void SteppedThroughSetup()
    {
        CpuMock.Registers.SP = 0xFC;
        Memory[0x01FD] = 0xFF;
        Memory[0x01FE] = 0x11;
        Memory[0x01FF] = 0x22;
    }

    public override void SteppedThroughVerification()
    {
        CpuMock.Registers.PC.Should().Be(0x2211);
        CpuMock.Registers.SP.Should().Be(0xFF);
        CpuMock.Flags.GetSR().Should().Be(0xFF);
    }
}