namespace Emu6502.Tests.Unit.Instructions;

public abstract class ROL_Tests : InstructionTestBase
{
    protected ROL_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void InstructionTestSetup(byte initialValue);
    protected abstract void InstructionTestVerification(byte expectedValue, byte expected_flags);

    [Theory]                       //NV-BDIZC
    [InlineData(0x00, 0x00, false, 0b00110110)]
    [InlineData(0x01, 0x02, false, 0b00110100)]
    [InlineData(0x40, 0x80, false, 0b10110100)]
    [InlineData(0x80, 0x00, false, 0b00110111)]
    [InlineData(0x24, 0x48, false, 0b00110100)]

    [InlineData(0x00, 0x01, true, 0b00110100)]
    [InlineData(0x01, 0x03, true, 0b00110100)]
    [InlineData(0x40, 0x81, true, 0b10110100)]
    [InlineData(0x80, 0x01, true, 0b00110101)]
    [InlineData(0x24, 0x49, true, 0b00110100)]
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

    public class Accumulator : ROL_Tests
    {
        public Accumulator(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new ROL_Accumulator();

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

    public class Zeropage : ROL_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new ROL_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x74;

            Memory[0x0074] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0074].Should().Be(0xAA);
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

    public class ZeropageX : ROL_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new ROL_ZeropageX();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x01;
            Memory[0x0000] = 0x74;

            Memory[0x0075] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0075].Should().Be(0xAA);
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

    public class Absolute : ROL_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new ROL_Absolute();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x74;
            Memory[0x0001] = 0x54;

            Memory[0x5474] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x5474].Should().Be(0xAA);
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

    public class AbsoluteX : ROL_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 6;

        protected override Instruction Sut { get; } = new ROL_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x01;
            Memory[0x0000] = 0x74;
            Memory[0x0001] = 0x54;

            Memory[0x5475] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x5475].Should().Be(0xAA);
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
