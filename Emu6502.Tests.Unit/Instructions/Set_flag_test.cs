using Emu6502.Instructions;
using System.Linq.Expressions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class Set_flag_test : InstructionTestBase
{
    public override int NumberOfCyclesForExecution => 1;
    public abstract Expression<Func<ICpu, bool>> AffectedBit { get; }

    [Fact]
    public void Should_set_specified_flag()
    {
        CpuMock.Flags.C = false;
        CpuMock.Flags.D = false;
        CpuMock.Flags.I = false;
        CpuMock.Flags.V = false;

        Sut.Execute(CpuMock);

        AffectedBit.Compile()(CpuMock).Should().BeTrue();
    }

    [Fact]
    public void Should_not_affect_other_flags()
    {
        CpuMock.Flags.C = false;
        CpuMock.Flags.D = false;
        CpuMock.Flags.I = false;
        CpuMock.Flags.V = false;

        Sut.Execute(CpuMock);

        CpuMock.ActiveFlags().Should().Be(1);
    }

    public class SEC_tests : Set_flag_test
    {
        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.C;

        protected override Instruction Sut { get; } = new SEC();
    }

    public class SED_tests : Set_flag_test
    {
        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.D;
        protected override Instruction Sut { get; } = new SED();
    }

    public class SEI_tests : Set_flag_test
    {
        public override Expression<Func<ICpu, bool>> AffectedBit => cpu => cpu.Flags.I;
        protected override Instruction Sut { get; } = new SEI();
    }
}
