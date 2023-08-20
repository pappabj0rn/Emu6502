namespace Emu6502.Tests.Unit.Instructions;

public abstract class STA_Tests : InstructionTestBase
{
    public STA_Tests(ITestOutputHelper output) : base(output) { }
    
    public class Zeropage_tests : STA_Tests
    {
        public Zeropage_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;

        protected override Instruction Sut { get; } = new STA_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x47;
            CpuMock.Registers.A = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0047].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }

    public class ZeropageX_tests : STA_Tests
    {
        public ZeropageX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new STA_ZeropageX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x47;
            CpuMock.Registers.A = 0x56;
            CpuMock.Registers.X = 0x05;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x004C].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }

    public class Absolute_tests : STA_Tests
    {
        public Absolute_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new STA_Absolute();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xAA;
            Memory[0x0001] = 0xBB;
            CpuMock.Registers.A = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xBBAA].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }

    public class AbsoluteX_tests : STA_Tests
    {
        public AbsoluteX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new STA_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xAA;
            Memory[0x0001] = 0xBB;
            CpuMock.Registers.A = 0x56;
            CpuMock.Registers.X = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xBBBB].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }

    public class AbsoluteY_tests : STA_Tests
    {
        public AbsoluteY_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;

        protected override Instruction Sut { get; } = new STA_AbsoluteY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xAA;
            Memory[0x0001] = 0xBB;
            CpuMock.Registers.A = 0x56;
            CpuMock.Registers.Y = 0x22;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xBBCC].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }

    public class IndirectX_tests : STA_Tests
    {
        public IndirectX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new STA_IndirectX();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x02;
            Memory[0x0000] = 0x11;

            Memory[0x0013] = 0x77;
            Memory[0x0014] = 0x88;

            CpuMock.Registers.A = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x8877].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }

    public class IndirectY_tests : STA_Tests
    {
        public IndirectY_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;

        protected override Instruction Sut { get; } = new STA_IndirectY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x11;

            Memory[0x0011] = 0x77;
            Memory[0x0012] = 0x88;

            CpuMock.Registers.A = 0x56;
            CpuMock.Registers.Y = 0x22;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x8899].Should().Be(0x56);
            CpuMock.Registers.A.Should().Be(0x56);
        }
    }
}

public abstract class STX_Tests : InstructionTestBase
{
    public STX_Tests(ITestOutputHelper output) : base(output) { }

    public class Zeropage_tests : STX_Tests
    {
        public Zeropage_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;

        protected override Instruction Sut { get; } = new STX_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x47;
            CpuMock.Registers.X = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0047].Should().Be(0x56);
            CpuMock.Registers.X.Should().Be(0x56);
        }
    }

    public class ZeropageY_tests : STX_Tests
    {
        public ZeropageY_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new STX_ZeropageY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x47;
            CpuMock.Registers.X = 0x56;
            CpuMock.Registers.Y = 0x05;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x004C].Should().Be(0x56);
            CpuMock.Registers.X.Should().Be(0x56);
        }
    }

    public class Absolute_tests : STX_Tests
    {
        public Absolute_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new STX_Absolute();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xAA;
            Memory[0x0001] = 0xBB;
            CpuMock.Registers.X = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xBBAA].Should().Be(0x56);
            CpuMock.Registers.X.Should().Be(0x56);
        }
    }

}

public abstract class STY_Tests : InstructionTestBase
{
    public STY_Tests(ITestOutputHelper output) : base(output) { }

    public class Zeropage_tests : STY_Tests
    {
        public Zeropage_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;

        protected override Instruction Sut { get; } = new STY_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x47;
            CpuMock.Registers.Y = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0047].Should().Be(0x56);
            CpuMock.Registers.Y.Should().Be(0x56);
        }
    }

    public class ZeropageX_tests : STY_Tests
    {
        public ZeropageX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new STY_ZeropageX();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.X = 0x11;
            Memory[0x0000] = 0x47;

            CpuMock.Registers.Y = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x0058].Should().Be(0x56);
            CpuMock.Registers.Y.Should().Be(0x56);
        }
    }

    public class Absolute_tests : STY_Tests
    {
        public Absolute_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new STY_Absolute();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0xAA;
            Memory[0x0001] = 0xBB;
            CpuMock.Registers.Y = 0x56;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0xBBAA].Should().Be(0x56);
            CpuMock.Registers.Y.Should().Be(0x56);
        }
    }
}