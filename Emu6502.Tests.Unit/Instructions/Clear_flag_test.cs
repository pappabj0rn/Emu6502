using Emu6502.Instructions;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class Clear_flag_test : InstructionTestBase
{
    public Clear_flag_test(ITestOutputHelper output) : base(output) { }

    public override int NumberOfCyclesForExecution => 1;
    public abstract Expression<Func<ICpu, bool>> AffectedBit { get; }

    [Fact]
    public void Should_clear_specified_flag()
    {
        CpuMock.Flags.C = true;
        CpuMock.Flags.D = true;
        CpuMock.Flags.I = true;
        CpuMock.Flags.V = true;

        Sut.Execute(CpuMock);

        AffectedBit.Compile()(CpuMock).Should().BeFalse();
    }

    [Fact]
    public void Should_not_affect_other_flags()
    {
        CpuMock.Flags.C = true;
        CpuMock.Flags.D = true;
        CpuMock.Flags.I = true;
        CpuMock.Flags.V = true;

        Sut.Execute(CpuMock);

        CpuMock.ActiveFlags().Should().Be(3);
    }

    public class CLC_tests : Clear_flag_test
    {
        public CLC_tests(ITestOutputHelper output) : base(output) { }

        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.C;

        protected override Instruction Sut { get; } = new CLC();
    }

    public class CLD_tests : Clear_flag_test
    {
        public CLD_tests(ITestOutputHelper output) : base(output) { }

        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.D;
        protected override Instruction Sut { get; } = new CLD();
    }

    public class CLI_tests : Clear_flag_test
    {
        public CLI_tests(ITestOutputHelper output) : base(output) { }

        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.I;
        protected override Instruction Sut { get; } = new CLI();
    }

    public class CLV_tests : Clear_flag_test
    {
        public CLV_tests(ITestOutputHelper output) : base(output) { }

        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.V;
        protected override Instruction Sut { get; } = new CLV();
    }
}
