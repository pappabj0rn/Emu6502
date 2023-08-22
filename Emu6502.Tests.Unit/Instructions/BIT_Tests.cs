namespace Emu6502.Tests.Unit.Instructions;

public abstract class BIT_Tests : InstructionTestBase
{
    protected BIT_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void BitOperationTestsSetup(byte mem);

    [Theory]                //NV-BDIZC
    [InlineData(0x00, 0x00, 0b00110110)]
    [InlineData(0x00, 0x01, 0b00110110)]
    [InlineData(0x01, 0x00, 0b00110110)]
    [InlineData(0x01, 0x01, 0b00110100)]
    [InlineData(0x10, 0x01, 0b00110110)]
    [InlineData(0x40, 0x40, 0b01110100)]
    [InlineData(0x80, 0x80, 0b10110100)]
    [InlineData(0x40, 0xC0, 0b11110100)]
    public void Bit_operation_tests(
        byte a,
        byte mem,
        byte expected_flags)
    {
        CpuMock.Registers.A = a;
        BitOperationTestsSetup(mem);

        Sut.Execute(CpuMock);

        CpuMock.Flags.GetSR().Should().Be(expected_flags);
    }

    public class Zeropage : BIT_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;

        protected override Instruction Sut { get; } = new BIT_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x08;

            Memory[0x0008] = 0xC0;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Flags.N.Should().BeTrue();
            CpuMock.Flags.V.Should().BeTrue();
            CpuMock.Flags.Z.Should().BeTrue();
        }

        protected override void BitOperationTestsSetup(byte mem)
        {
            Memory[0x0000] = 0x08;

            Memory[0x0008] = mem;
        }
    }

    public class Absolute : BIT_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new BIT_Absolute();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x08;
            Memory[0x0001] = 0x00;

            Memory[0x0008] = 0xC0;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Flags.N.Should().BeTrue();
            CpuMock.Flags.V.Should().BeTrue();
            CpuMock.Flags.Z.Should().BeTrue();
        }

        protected override void BitOperationTestsSetup(byte mem)
        {
            Memory[0x0000] = 0x10;
            Memory[0x0001] = 0x10;

            Memory[0x1010] = mem;
        }
    }
}
