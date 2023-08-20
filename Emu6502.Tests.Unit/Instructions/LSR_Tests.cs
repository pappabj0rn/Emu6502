namespace Emu6502.Tests.Unit.Instructions;
/*
Shift One Bit Right (Memory or Accumulator)

0 -> [76543210] -> C
                        N	Z	C	I	D	V
                        0	+	+	-	-	-
addressing	assembler	opc	bytes	cycles
accumulator	LSR A	    4A	1	    1  
zeropage	LSR oper	46	2	    4  
zeropage,X	LSR oper,X	56	2	    5  
absolute	LSR oper	4E	3	    6  
absolute,X	LSR oper,X	5E	3	    7  
 
 */
public abstract class LSR_Tests : InstructionTestBase
{
    protected LSR_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void InstructionTestSetup(byte initialValue);
    protected abstract void InstructionTestVerification(byte expectedValue, byte expected_flags);

    [Theory]                //NV-BDIZC
    [InlineData(0x00, 0x00, 0b00110110)]
    [InlineData(0x01, 0x00, 0b00110111)]
    [InlineData(0x02, 0x01, 0b00110100)]
    [InlineData(0x40, 0x20, 0b00110100)]
    [InlineData(0x80, 0x40, 0b00110100)]
    [InlineData(0x48, 0x24, 0b00110100)]
    public void Should_shift_target_let_one_step(
            byte initialValue,
            byte expectedValue,
            byte expected_flags)
    {
        InstructionTestSetup(initialValue);

        Sut.Execute(CpuMock);

        InstructionTestVerification(expectedValue, expected_flags);
    }

    public class Accumulator : LSR_Tests
    {
        public Accumulator(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new LSR_Accumulator();

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

    public class Zeropage : LSR_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new LSR_Zeropage();

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

    public class ZeropageX : LSR_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new LSR_ZeropageX();

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

    public class Absolute : LSR_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new LSR_Absolute();

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

    public class AbsoluteX : LSR_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 6;

        protected override Instruction Sut { get; } = new LSR_AbsoluteX();

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
