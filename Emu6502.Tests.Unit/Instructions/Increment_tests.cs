using Emu6502.Instructions;
using Xunit.Abstractions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class Increment_tests : InstructionTestBase
{
    public Increment_tests(ITestOutputHelper output) : base(output) { }
    public abstract void SetupTestMemory(byte value);
    public abstract void VerifyMemory(byte expected);

    [Theory]
    [InlineData(0x00, 0x01)]
    [InlineData(0x01, 0x02)]
    [InlineData(0xFF, 0x00)]
    public void Should_increment_memory_at_adress(byte initialValue, byte expectedValue)
    {
        SetupTestMemory(initialValue);

        Sut.Execute(CpuMock);

        VerifyMemory(expectedValue);
    }

    public class INC_Absoulte_tests : Increment_tests
    {
        public INC_Absoulte_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new INC_Absolute();

        public override void SetupTestMemory(byte value)
        {
            Memory[0x0000] = 0x34;
            Memory[0x0001] = 0x12;
            Memory[0x1234] = value;
        }

        public override void VerifyMemory(byte expected)
        {
            Memory[0x1234].Should().Be(expected);
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xCD;
            Memory[0x0001] = 0xAB;

            Memory[0xABCD] = 0xCD;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xABCD].Should().Be(0xCE);
        }
    }

    public class INC_AbsoulteX_tests : Increment_tests
    {
        public INC_AbsoulteX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 6;

        protected override Instruction Sut { get; } = new INC_AbsoluteX();

        public override void SetupTestMemory(byte value)
        {
            CpuMock.Registers.X = 0x04;

            Memory[0x0000] = 0x34;
            Memory[0x0001] = 0x12;

            Memory[0x1238] = value;
        }

        public override void VerifyMemory(byte expected)
        {
            Memory[0x1238].Should().Be(expected);
        }

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x01;

            Memory[0x0000] = 0xCD;
            Memory[0x0001] = 0xAB;

            Memory[0xABCE] = 0xCD;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xABCE].Should().Be(0xCE);
        }
    }

    public class INC_Zeropage_tests : Increment_tests
    {
        public INC_Zeropage_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new INC_Zeropage();

        public override void SetupTestMemory(byte value)
        {
            Memory[0x0000] = 0x01;
            Memory[0x0001] = value;
        }

        public override void VerifyMemory(byte expected)
        {
            Memory[0x0001].Should().Be(expected);
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xCD;

            Memory[0x00CD] = 0xAB;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x00CD].Should().Be(0xAC);
        }
    }

    public class INC_ZeropageX_tests : Increment_tests
    {
        public INC_ZeropageX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new INC_ZeropageX();

        public override void SetupTestMemory(byte value)
        {
            CpuMock.Registers.X = 0x02;

            Memory[0x0000] = 0x34;
            Memory[0x0036] = value;
        }

        public override void VerifyMemory(byte expected)
        {
            Memory[0x0036].Should().Be(expected);
        }

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x01;

            Memory[0x0000] = 0x00;
            Memory[0x0001] = 0xAB;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0001].Should().Be(0xAC);
        }
    }

    public class INX_tests : Increment_tests
    {
        public INX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new INX();

        public override void SetupTestMemory(byte value)
        {
            CpuMock.Registers.X = value;
        }

        public override void VerifyMemory(byte expected)
        {
            CpuMock.Registers.X.Should().Be(expected);
        }

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0xEF;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.X.Should().Be(0xF0);
        }
    }

    public class INY_tests : Increment_tests
    {
        public INY_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new INY();

        public override void SetupTestMemory(byte value)
        {
            CpuMock.Registers.Y = value;
        }

        public override void VerifyMemory(byte expected)
        {
            CpuMock.Registers.Y.Should().Be(expected);
        }

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.Y = 0xEF;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.Y.Should().Be(0xF0);
        }
    }
}
