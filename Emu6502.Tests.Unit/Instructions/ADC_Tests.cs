using Emu6502.Instructions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class ADC_Tests : InstructionTestBase
{
    protected abstract void ADC_instruction_test_memory_setup(ICpu cpu, byte expectedValue);

    [Theory]
    [InlineData(0x00, 0x01, false, 0x01, false, false, false)]
    [InlineData(0x01, 0x01, false, 0x02, false, false, false)]
    [InlineData(0x00, 0xFF, false, 0xFF, false, true, false)]
    [InlineData(0x01, 0xFF, false, 0x00, true, false, true)]
    [InlineData(0x7F, 0x01, false, 0x80, false, true, false)]
    [InlineData(0x80, 0x01, false, 0x81, false, true, false)]
    [InlineData(0xFF, 0x02, false, 0x01, false, false, true)]
    [InlineData(0x00, 0x01, true, 0x02, false, false, false)]
    [InlineData(0x01, 0x01, true, 0x03, false, false, false)]
    [InlineData(0x00, 0xFF, true, 0x00, true, false, true)]
    [InlineData(0x01, 0xFF, true, 0x01, false, false, true)]
    [InlineData(0x7F, 0x01, true, 0x81, false, true, false)]
    [InlineData(0x80, 0x01, true, 0x82, false, true, false)]
    [InlineData(0xFF, 0x02, true, 0x02, false, false, true)]
    public void Should_update_Z_and_N_flags_matching_value_loaded_into_accumulator(
        byte inital_a,
        byte memory,
        bool initial_c,
        byte expected_a,
        bool expected_z,
        bool expected_n,
        bool expected_c)
    {
        ADC_instruction_test_memory_setup(CpuMock, memory);

        CpuMock.Registers.A = inital_a;
        CpuMock.Flags.C = initial_c;

        Sut.Execute(CpuMock);

        CpuMock.Registers.A.Should().Be(expected_a);
        CpuMock.Flags.Z.Should().Be(expected_z);
        CpuMock.Flags.N.Should().Be(expected_n);
        CpuMock.Flags.C.Should().Be(expected_c);
    }

    public class Immediate : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new ADC_Immediate();

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = expectedValue;
        }
    }

    public class Absolute : ADC_Tests
    {
        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ADC_Absolute();

        [Fact]
        public void Should_load_byte_at_address_following_instruction_into_accumulator()
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0x45;

            Memory[0x4523] = 0x01;

            CpuMock.Flags.C = true;
            CpuMock.Registers.A = 0x01;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x03);
        }

        protected override void ADC_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = 0x12;
            Memory[0x0001] = 0x34;

            Memory[0x3412] = expectedValue;
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0x45;

            Memory[0x4523] = 0x55;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x55);
        }
    }
}
