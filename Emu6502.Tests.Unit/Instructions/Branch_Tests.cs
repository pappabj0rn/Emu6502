using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class Branch_Tests : InstructionTestBase
{
    protected Branch_Tests(ITestOutputHelper output) : base(output) { }

    public override int NumberOfCyclesForExecution => 2;

    public override void SteppedThroughSetup()
    {
        BranchTestConditionSetup(true);
        Memory[0x0000] = 0x01;
    }

    public override void SteppedThroughVerification()
    {
        CpuMock.Registers.PC.Should().Be(0x02);
    }

    public override void CycleCountSetup()
    {
        BranchTestConditionSetup(true);
    }

    public abstract void BranchTestConditionSetup(bool trigger);

    [Theory]
    [InlineData(true, 0x0005, 0x01, 0x0007)]
    [InlineData(true, 0x0005, 0xFD, 0x0004)]
    [InlineData(false, 0x0005, 0x05, 0x0006)]
    public void Should_branch_on_condition(
            bool trigger,
            ushort initial_pc,
            byte relative_addr,
            ushort expectd_pc)
    {
        BranchTestConditionSetup(trigger);
        CpuMock.Registers.PC = initial_pc;
        Memory[initial_pc] = relative_addr;

        Sut.Execute(CpuMock);

        CpuMock.Registers.PC.Should().Be(expectd_pc);
    }

    [Fact]
    public void Should_require_only_1_cycle_when_not_branching()
    {
        CpuMock.State.RemainingCycles = 1;

        BranchTestConditionSetup(false);
        Memory[0x0000] = 0x0A;

        Sut.Execute(CpuMock);

        CpuMock.Registers.PC.Should().Be(0x01);
        CpuMock.State.Ticks.Should().Be(1);
        CpuMock.State.Instruction.Should().BeNull();
    }

    [Theory]
    [InlineData(0x00FE, 0x01, 0x0100)]
    [InlineData(0x0100, 0xFD, 0x00FF)]
    public void Should_require_3_cycles_when_branching_to_other_page(
        ushort initial_pc,
        byte relative_addr,
        ushort expectd_pc)
    {
        CpuMock.State.RemainingCycles = 3;

        BranchTestConditionSetup(true);
        CpuMock.Registers.PC = initial_pc;
        Memory[initial_pc] = relative_addr;

        Sut.Execute(CpuMock);

        CpuMock.Registers.PC.Should().Be(expectd_pc);
        CpuMock.State.Ticks.Should().Be(3);
        CpuMock.State.Instruction.Should().BeNull();
    }

    public class BCC_Tests : Branch_Tests
    {
        public BCC_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BCC();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.C = !trigger;
        }
    }

    public class BCS_Tests : Branch_Tests
    {
        public BCS_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BCS();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.C = trigger;
        }
    }

    public class BEQ_Tests : Branch_Tests
    {
        public BEQ_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BEQ();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.Z = trigger;
        }
    }

    public class BMI_Tests : Branch_Tests
    {
        public BMI_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BMI();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.N = trigger;
        }
    }

    public class BNE_Tests : Branch_Tests
    {
        public BNE_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BNE();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.Z = !trigger;
        }
    }

    public class BPL_Tests : Branch_Tests
    {
        public BPL_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BPL();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.N = !trigger;
        }
    }

    public class BVC_Tests : Branch_Tests
    {
        public BVC_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BVC();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.V = !trigger;
        }
    }

    public class BVS_Tests : Branch_Tests
    {
        public BVS_Tests(ITestOutputHelper output) : base(output) { }

        protected override Instruction Sut { get; } = new BVS();

        public override void BranchTestConditionSetup(bool trigger)
        {
            CpuMock.Flags.V = trigger;
        }
    }
}
