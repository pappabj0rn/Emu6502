using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class TransferInstructions_tests : InstructionTestBase
{
    public class TAX_tests : TransferInstructions_tests
    {
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
        public override int NumberOfCyclesForExecution => 1;

        protected override Instruction Sut { get; } = new TXS();

        [Theory]         //NV BDIZC
        [InlineData(0x00, 0b00000110)]
        [InlineData(0x01, 0b00000100)]
        [InlineData(0x80, 0b10000100)]
        public void Should_transfer_X_to_SP(
        byte inital,
        byte expected_flags)
        {
            CpuMock.Registers.X = inital;

            Sut.Execute(CpuMock);

            CpuMock.Registers.SP.Should().Be(inital);
            VerifyFlags(expected_flags);
        }
    }

    public class TYA_tests : TransferInstructions_tests
    {
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
