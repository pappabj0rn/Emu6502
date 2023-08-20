using Emu6502.Instructions;
using Xunit.Abstractions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class ASL_Tests : InstructionTestBase
{
    protected ASL_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void AslTestSetup(byte initialValue);
    protected abstract void AslTestVerification(byte expectedValue, byte expected_flags);

    [Theory]                //NV-BDIZC
    [InlineData(0x00, 0x00, 0b00110110)]
    [InlineData(0x01, 0x02, 0b00110100)]
    [InlineData(0x40, 0x80, 0b10110100)]
    [InlineData(0x80, 0x00, 0b00110111)]
    [InlineData(0x24, 0x48, 0b00110100)]
    public void Should_shift_target_let_one_step(
            byte initialValue,
            byte expectedValue,
            byte expected_flags)
    {
        AslTestSetup(initialValue);

        Sut.Execute(CpuMock);

        AslTestVerification(expectedValue, expected_flags);
    }

    public class Accumulator : ASL_Tests
    {
        public Accumulator(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new ASL_Accumulator();

        protected override void AslTestSetup(byte initialValue)
        {
            CpuMock.Registers.A = initialValue;
        }

        protected override void AslTestVerification(byte expectedValue, byte expected_flags)
        {
            CpuMock.Registers.A.Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class Zeropage : ASL_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new ASL_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x74;

            Memory[0x0074] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0074].Should().Be(0xAA);
        }

        protected override void AslTestSetup(byte initialValue)
        {
            Memory[0x0000] = 0x33;

            Memory[0x0033] = initialValue;
        }

        protected override void AslTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x0033].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class ZeropageX : ASL_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new ASL_ZeropageX();

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

        protected override void AslTestSetup(byte initialValue)
        {
            CpuMock.Registers.X = 0x01;
            Memory[0x0000] = 0x33;

            Memory[0x0034] = initialValue;
        }

        protected override void AslTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x0034].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class Absolute : ASL_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new ASL_Absolute();

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

        protected override void AslTestSetup(byte initialValue)
        {
            Memory[0x0000] = 0x33;
            Memory[0x0001] = 0x44;

            Memory[0x4433] = initialValue;
        }

        protected override void AslTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x4433].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }

    public class AbsoluteX : ASL_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 6;

        protected override Instruction Sut { get; } = new ASL_AbsoluteX();

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

        protected override void AslTestSetup(byte initialValue)
        {
            CpuMock.Registers.X = 0x11;
            Memory[0x0000] = 0x33;
            Memory[0x0001] = 0x44;

            Memory[0x4444] = initialValue;
        }

        protected override void AslTestVerification(byte expectedValue, byte expected_flags)
        {
            Memory[0x4444].Should().Be(expectedValue);
            CpuMock.Flags.GetSR().Should().Be(expected_flags);
        }
    }
}
