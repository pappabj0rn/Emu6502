namespace Emu6502.Tests.Unit.Instructions;

public abstract class ROR_Tests : InstructionTestBase
{
    protected ROR_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void InstructionTestSetup(byte initialValue);
    protected abstract void InstructionTestVerification(byte expectedValue, byte expected_flags);

    [Theory]                       //NV-BDIZC
    [InlineData(0x00, 0x00, false, 0b00110110)]
    [InlineData(0x01, 0x00, false, 0b00110111)]
    [InlineData(0x02, 0x01, false, 0b00110100)]
    [InlineData(0x40, 0x20, false, 0b00110100)]
    [InlineData(0x80, 0x40, false, 0b00110100)]
    [InlineData(0x48, 0x24, false, 0b00110100)]

    [InlineData(0x00, 0x80, true,  0b10110100)]
    [InlineData(0x01, 0x80, true,  0b10110101)]
    [InlineData(0x02, 0x81, true,  0b10110100)]
    [InlineData(0x40, 0xA0, true,  0b10110100)]
    [InlineData(0x80, 0xC0, true,  0b10110100)]
    [InlineData(0x48, 0xA4, true,  0b10110100)]
    public void Should_shift_target_let_one_step(
            byte initialValue,
            byte expectedValue,
            bool initial_C,
            byte expected_flags)
    {
        CpuMock.Flags.C = initial_C;
        InstructionTestSetup(initialValue);

        Sut.Execute(CpuMock);

        InstructionTestVerification(expectedValue, expected_flags);
    }

    public class Accumulator : ROR_Tests
    {
        public Accumulator(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new ROR_Accumulator();

        protected override void InstructionTestSetup(byte initialValue)
        {
            CpuMock.Registers.A = initialValue;
        }

        protected override void InstructionTestVerification(byte expectedValue, byte expected_flags)
        {
            CpuMock.Registers.A.Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class Zeropage : ROR_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new ROR_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x74;

            Memory[0x0074] = 0xAA;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0074].Should().Be(0x55);
        }

        protected override void InstructionTestSetup(byte initialValue)
        {
            Memory[0x0000] = 0x33;

            Memory[0x0033] = initialValue;
        }

        protected override void InstructionTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x0033].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class ZeropageX : ROR_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new ROR_ZeropageX();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x01;
            Memory[0x0000] = 0x74;

            Memory[0x0075] = 0xAA;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0075].Should().Be(0x55);
        }

        protected override void InstructionTestSetup(byte initialValue)
        {
            CpuMock.Registers.X = 0x01;
            Memory[0x0000] = 0x33;

            Memory[0x0034] = initialValue;
        }

        protected override void InstructionTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x0034].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class Absolute : ROR_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new ROR_Absolute();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x74;
            Memory[0x0001] = 0x54;

            Memory[0x5474] = 0xAA;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x5474].Should().Be(0x55);
        }

        protected override void InstructionTestSetup(byte initialValue)
        {
            Memory[0x0000] = 0x33;
            Memory[0x0001] = 0x44;

            Memory[0x4433] = initialValue;
        }

        protected override void InstructionTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x4433].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class AbsoluteX : ROR_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 6;

        protected override Instruction Sut { get; } = new ROR_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x01;
            Memory[0x0000] = 0x74;
            Memory[0x0001] = 0x54;

            Memory[0x5475] = 0xAA;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x5475].Should().Be(0x55);
        }

        protected override void InstructionTestSetup(byte initialValue)
        {
            CpuMock.Registers.X = 0x11;
            Memory[0x0000] = 0x33;
            Memory[0x0001] = 0x44;

            Memory[0x4444] = initialValue;
        }

        protected override void InstructionTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x4444].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }
}
