using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class StackInstructions_tests : InstructionTestBase
{
    public class PHA_tests : StackInstructions_tests
    {
        public override int NumberOfCyclesForExecution => 2;

        protected override Instruction Sut { get; } = new PHA();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.A = 0x12;
            CpuMock.Registers.SP = 0xFF;
        }

        public override void SteppedThroughVerification()
        {
            Memory[0x01FF].Should().Be(0x12);
            CpuMock.Registers.SP.Should().Be(0xFE);
        }

        [Theory]
        [InlineData(0x01, 0xFF, 0xFE)]
        [InlineData(0xFF, 0xFE, 0xFD)]
        public void Should_push_accumulator_to_the_stack_and_update_stackpointer(
            byte initial_a,
            byte initial_sp,
            byte expected_sp)
        {
            CpuMock.Registers.A = initial_a;
            CpuMock.Registers.SP = initial_sp;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(initial_a);
            Memory[0x0100 | initial_sp].Should().Be(initial_a);
            CpuMock.Registers.SP.Should().Be(expected_sp);
        }
    }

    public class PLA_tests : StackInstructions_tests
    {
        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new PLA();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.A = 0x12;
            CpuMock.Registers.SP = 0xFE;
            Memory[0x01FF] = 0x34;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x34);
            CpuMock.Registers.SP = 0xFF;
        }

        [Theory]                           //nvdizc
        [InlineData(0x00, 0x01, 0xFE, 0xFF, "000100")]
        [InlineData(0x00, 0x00, 0xFE, 0xFF, "000110")]
        [InlineData(0x00, 0xFF, 0xFD, 0xFE, "100100")]
        public void Should_pull_accumulator_from_the_stack_and_update_stackpointer(
            byte initial_a,
            byte initial_mem_in_stack,
            byte initial_sp,
            byte expected_sp,
            string expected_flags)
        {
            CpuMock.Registers.A = initial_a;
            CpuMock.Registers.SP = initial_sp;
            Memory[0x0100 | expected_sp] = initial_mem_in_stack;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(initial_mem_in_stack);
            Memory[0x0100 | expected_sp].Should().Be(initial_mem_in_stack);
            CpuMock.Registers.SP.Should().Be(expected_sp);
            VerifyFlags(expected_flags);
        }
    }

    public class PHP_tests : StackInstructions_tests
    {
        public override int NumberOfCyclesForExecution => 2;

        protected override Instruction Sut { get; } = new PHP();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.SP = 0xFF;
            CpuMock.Flags.N = true;
            CpuMock.Flags.V = true;
            CpuMock.Flags.B = true;
            CpuMock.Flags.D = true;
            CpuMock.Flags.I = true;
            CpuMock.Flags.Z = false;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.SP = 0xFE;
            Memory[0x01FF].Should().Be(0xFD);
        }

        [Theory]                //nv b dizc, bit [6:5] will allways be set
        [InlineData(0xFF, 0xFE, 0b0000_0000, 0b0011_0000)]
        [InlineData(0xFF, 0xFE, 0b0000_0001, 0b0011_0001)]
        [InlineData(0xFF, 0xFE, 0b0000_0010, 0b0011_0010)]
        [InlineData(0xFF, 0xFE, 0b0000_0100, 0b0011_0100)]
        [InlineData(0xFF, 0xFE, 0b0000_1000, 0b0011_1000)]
        [InlineData(0xFF, 0xFE, 0b0100_0000, 0b0111_0000)]
        [InlineData(0xFF, 0xFE, 0b1000_0000, 0b1011_0000)]
        [InlineData(0xFF, 0xFE, 0b1100_1111, 0b1111_1111)]
        public void Should_push_status_register_to_the_stack_and_update_stackpointer(
            byte initial_sp,
            byte expected_sp,
            byte initial_flags,
            byte expected_flags_byte)
        {
            CpuMock.Registers.SP = initial_sp;
            CpuMock.Flags.SetSR(initial_flags);

            Sut.Execute(CpuMock);

            Memory[0x0100 | initial_sp].Should().Be(expected_flags_byte);
            CpuMock.Registers.SP.Should().Be(expected_sp);
            VerifyFlags((byte)(initial_flags | 0x30));
        }
    }

    public class PLP_tests : StackInstructions_tests
    {
        public override int NumberOfCyclesForExecution => 3;

        protected override Instruction Sut { get; } = new PLP();

        public override void SteppedThroughSetup()
        {
            CpuMock.Registers.SP = 0xFE;
            Memory[0x01FF] = 0xFF;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.SP = 0xFF;
            CpuMock.Flags.N.Should().BeTrue();
            CpuMock.Flags.V.Should().BeTrue();
            CpuMock.Flags.D.Should().BeTrue();
            CpuMock.Flags.I.Should().BeTrue();
            CpuMock.Flags.Z.Should().BeTrue();
            CpuMock.Flags.C.Should().BeTrue();
        }

        [Theory]                //nv bdizc, bit [6:5] will be ignored
        [InlineData(0xFE, 0xFF, 0b00110000, 0b00000000)]
        [InlineData(0xFE, 0xFF, 0b11111111, 0b11001111)]
        public void Should_push_status_register_to_the_stack_and_update_stackpointer(
            byte initial_sp,
            byte expected_sp,
            byte initial_mem_in_stack,
            byte expected_flags_byte)
        {
            CpuMock.Registers.SP = initial_sp;
            Memory[0x0100 | expected_sp] = initial_mem_in_stack;

            Sut.Execute(CpuMock);

            Memory[0x0100 | expected_sp].Should().Be(initial_mem_in_stack);
            CpuMock.Registers.SP.Should().Be(expected_sp);
            VerifyFlags(expected_flags_byte);
        }
    }
}
