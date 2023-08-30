namespace Emu6502.Tests.Unit.Instructions;

public class BRK_Tests : InstructionTestBase
{
    public BRK_Tests(ITestOutputHelper output) : base(output) { }

    public override int NumberOfCyclesForExecution => 6;

    protected override Instruction Sut { get; } = new BRK();

    public override void SteppedThroughSetup()
    {
        CpuMock.Flags.C = true;
        Memory[0x1234] = CpuMock.Flags.GetSR();

        Memory[0xFFFE] = 0x11;
        Memory[0xFFFF] = 0x22;
    }

    public override void SteppedThroughVerification()
    {
        CpuMock.Registers.PC.Should().Be(0x2211);
        CpuMock.Registers.SP.Should().Be(0xFC);

        Memory[0x01FD].Should().Be(Memory[0x1234]);
        Memory[0x01FE].Should().Be(0x01);
        Memory[0x01FF].Should().Be(0x00);
    }

    [Fact]
    public void Should_set_interrupt_disable_falg()
    {
        CpuMock.Flags.I = false;

        Sut.Execute(CpuMock);

        CpuMock.Flags.I.Should().BeTrue();
    }

}
