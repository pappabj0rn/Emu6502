namespace Emu6502.Tests.Unit.Instructions;

public abstract class TransferInstructions_tests : InstructionTestBase
{
    public TransferInstructions_tests(ITestOutputHelper output) : base(output) { }

    public class TAX_tests : TransferInstructions_tests
    {
        public TAX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TAX();

        [Theory]         //NV BDIZC
        [InlineData(0x00, 0b00000110)]
        [InlineData(0x01, 0b00000100)]
        [InlineData(0x80, 0b10000100)]
        public void Should_transfer_A_to_X(
        byte inital,
        byte expected_flags)
        {
            CpuMock.Registers.A = inital;

            Sut.Execute(CpuMock);

            CpuMock.Registers.X.Should().Be(inital);
            VerifyFlags(expected_flags);
        }
    }

    public class TAY_tests : TransferInstructions_tests
    {
        public TAY_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TAY();

        [Theory]         //NV BDIZC
        [InlineData(0x00, 0b00000110)]
        [InlineData(0x01, 0b00000100)]
        [InlineData(0x80, 0b10000100)]
        public void Should_transfer_A_to_Y(
        byte inital,
        byte expected_flags)
        {
            CpuMock.Registers.A = inital;

            Sut.Execute(CpuMock);

            CpuMock.Registers.Y.Should().Be(inital);
            VerifyFlags(expected_flags);
        }
    }

    public class TSX_tests : TransferInstructions_tests
    {
        public TSX_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TSX();

        [Theory]         //NV BDIZC
        [InlineData(0x00, 0b00000110)]
        [InlineData(0x01, 0b00000100)]
        [InlineData(0x80, 0b10000100)]
        public void Should_transfer_SP_to_X(
        byte inital,
        byte expected_flags)
        {
            CpuMock.Registers.SP = inital;

            Sut.Execute(CpuMock);

            CpuMock.Registers.X.Should().Be(inital);
            VerifyFlags(expected_flags);
        }
    }

    public class TXA_tests : TransferInstructions_tests
    {
        public TXA_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TXA();

        [Theory]         //NV BDIZC
        [InlineData(0x00, 0b00000110)]
        [InlineData(0x01, 0b00000100)]
        [InlineData(0x80, 0b10000100)]
        public void Should_transfer_X_to_A(
        byte inital,
        byte expected_flags)
        {
            CpuMock.Registers.X = inital;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(inital);
            VerifyFlags(expected_flags);
        }
    }

    public class TXS_tests : TransferInstructions_tests
    {
        public TXS_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TXS();

        [Theory]
        [InlineData(0x00)]
        [InlineData(0x01)]
        [InlineData(0x7E)]
        [InlineData(0x80)]
        [InlineData(0xFF)]
        public void Should_transfer_X_to_SP(byte inital)
        {
            CpuMock.Registers.X = inital;
            var initialFlags = CpuMock.Flags.GetSR();

            Sut.Execute(CpuMock);

            CpuMock.Registers.SP.Should().Be(inital);
            VerifyFlags(initialFlags);
        }
    }

    public class TYA_tests : TransferInstructions_tests
    {
        public TYA_tests(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TYA();

        [Theory]         //NV BDIZC
        [InlineData(0x00, 0b00000110)]
        [InlineData(0x01, 0b00000100)]
        [InlineData(0x80, 0b10000100)]
        public void Should_transfer_Y_to_A(
        byte inital,
        byte expected_flags)
        {
            CpuMock.Registers.Y = inital;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(inital);
            VerifyFlags(expected_flags);
        }
    }
}
