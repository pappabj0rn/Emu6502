namespace Emu6502.Tests.Unit.Instructions;

public abstract class JMP_Tests : InstructionTestBase
{
    public JMP_Tests(ITestOutputHelper output) : base(output) { }

    public class Absolute : JMP_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new JMP_Absolute();

        [Fact]
        public void Should_set_pc_to_address_following_instruction()
        {
            Memory[0x0000] = 0x11;
            Memory[0x0001] = 0x22;
            
            Sut.Execute(CpuMock);

            CpuMock.Registers.PC.Should().Be(0x2211);
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x12;
            Memory[0x0001] = 0x23;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.PC.Should().Be(0x2312);
        }
    }    

    public class Indirect : JMP_Tests
    {
        public Indirect(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;
        protected override Instruction Sut { get; } = new JMP_Indirect();

        [Fact]
        public void Should_set_pc_to_address_following_instruction()
        {
            Memory[0x0000] = 0x11;
            Memory[0x0001] = 0x22;

            Memory[0x2211] = 0x33;
            Memory[0x2212] = 0x44;

            Sut.Execute(CpuMock);

            CpuMock.Registers.PC.Should().Be(0x4433);
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x11;
            Memory[0x0001] = 0x45;

            Memory[0x4511] = 0x12;
            Memory[0x4512] = 0x23;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.PC.Should().Be(0x2312);
        }
    }
}
