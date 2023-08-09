namespace Emu6502.Tests.Unit.Instructions;

public abstract class LDA : InstructionTestBase
{
    public class Immediate : LDA
    {
        [Fact]
        public void Should_execute_in_two_cycles()
        {
            Memory[0x00] = Cpu.Instructions.LDA_Immediate;

            Cpu.Execute(2);
            Cpu.Ticks.Should().Be(2);
            Cpu.Registers.PC.Should().Be(0x02);
        }

        [Fact]
        public void Should_load_byte_following_instruction_into_accumulator()
        {
            Memory[0x00] = Cpu.Instructions.LDA_Immediate;
            Memory[0x01] = 0x01;

            Cpu.Execute(2);
            Cpu.Registers.A.Should().Be(0x01);
        }

        [Theory]
        [InlineData(0x00, true, false)]
        [InlineData(0x01, false, false)]
        [InlineData(0x7F, false, false)]
        [InlineData(0x80, false, true)]
        [InlineData(0x81, false, true)]
        [InlineData(0xFF, false, true)]
        public void Should_update_Z_and_N_flags_matching_value_loaded_into_accumulator(byte value, bool expected_z, bool expected_n)
        {
            Memory[0x0000] = Cpu.Instructions.LDA_Immediate;
            Memory[0x0001] = value;

            Cpu.Execute(2);
            Cpu.Registers.A.Should().Be(value);
            Cpu.Flags.Z.Should().Be(expected_z);
            Cpu.Flags.N.Should().Be(expected_n);
        }
    }
}
