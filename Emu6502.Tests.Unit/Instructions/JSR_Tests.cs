namespace Emu6502.Tests.Unit.Instructions;

public class JSR_Tests : InstructionTestBase
{
    public JSR_Tests(ITestOutputHelper output) : base(output) { }

    public override int NumberOfCyclesForExecution => 5;

    protected override Instruction Sut { get; } = new JSR_Absolute();

    public override void SteppedThroughSetup()
    {
        Memory[0x0000] = 0x11;
        Memory[0x0001] = 0x22;
    }

    public override void SteppedThroughVerification()
    {
        CpuMock.Registers.PC.Should().Be(0x2211);
        CpuMock.Registers.SP.Should().Be(0xFD);
        
        Memory[0x01FE].Should().Be(0x01);
        Memory[0x01FF].Should().Be(0x00);
    }
}
