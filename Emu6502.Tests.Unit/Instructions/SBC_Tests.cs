namespace Emu6502.Tests.Unit.Instructions;

public abstract class SBC_Tests : InstructionTestBase
{
    public SBC_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void SBC_instruction_test_memory_setup(ICpu cpu, byte operand);

    [Theory]//   a     op2    c      d     exp    nv-bdizc
    [InlineData(0x00, 0x00, false, false, 0xFF, 0b10110100)]
    [InlineData(0x00, 0x01, false, false, 0xFE, 0b10110100)]
    [InlineData(0x01, 0x01, false, false, 0xFF, 0b10110100)]
    [InlineData(0x00, 0xFF, false, false, 0x00, 0b00110110)]
    [InlineData(0x01, 0xFF, false, false, 0x01, 0b00110100)]
    [InlineData(0x7F, 0x01, false, false, 0x7D, 0b00110101)]
    [InlineData(0x80, 0x01, false, false, 0x7E, 0b01110101)]
    [InlineData(0xFF, 0x02, false, false, 0xFC, 0b10110101)]
    [InlineData(0x80, 0x80, false, false, 0xFF, 0b10110100)]
    [InlineData(0x7E, 0x7E, false, false, 0xFF, 0b10110100)]
    //                                            nv-bdizc
    [InlineData(0x00, 0x01, true,  false, 0xFF, 0b10110100)]
    [InlineData(0x01, 0x01, true,  false, 0x00, 0b00110111)]
    [InlineData(0x00, 0xFF, true,  false, 0x01, 0b00110100)]
    [InlineData(0x01, 0xFF, true,  false, 0x02, 0b00110100)]
    [InlineData(0x7F, 0x01, true,  false, 0x7E, 0b00110101)]
    [InlineData(0x80, 0x01, true,  false, 0x7F, 0b01110101)]
    [InlineData(0xFF, 0x02, true,  false, 0xFD, 0b10110101)]
    [InlineData(0x80, 0x80, true,  false, 0x00, 0b00110111)]
    [InlineData(0x7E, 0x7E, true,  false, 0x00, 0b00110111)]
    
    [InlineData(0x00, 0x00, true,  true,  0x00, 0b00111111)]
    [InlineData(0x01, 0x01, true,  true,  0x00, 0b00111111)]
    [InlineData(0x02, 0x01, true,  true,  0x01, 0b00111101)]
    [InlineData(0x10, 0x01, true,  true,  0x09, 0b00111101)]
    [InlineData(0x90, 0x10, true,  true,  0x80, 0b10111101)]
    [InlineData(0x01, 0x02, true,  true,  0x99, 0b10111100)]
    
    [InlineData(0x00, 0x00, false, true,  0x99, 0b10111100)]
    [InlineData(0x01, 0x01, false, true,  0x99, 0b10111100)]
    [InlineData(0x02, 0x01, false, true,  0x00, 0b00111111)]
    [InlineData(0x10, 0x01, false, true,  0x08, 0b00111101)]
    [InlineData(0x90, 0x10, false, true,  0x79, 0b00111101)]
    [InlineData(0x01, 0x02, false, true,  0x98, 0b10111100)]
    public void Should_update_Z_and_N_and_C_flags_matching_result_stored_in_accumulator(
       byte inital_a,
       byte memory,
       bool initial_c,
       bool d,
       byte expected_a,
       byte expected_flags)
    {
        SBC_instruction_test_memory_setup(CpuMock, memory);

        CpuMock.Registers.A = inital_a;
        CpuMock.Flags.C = initial_c;
        CpuMock.Flags.D = d;

        Sut.Execute(CpuMock);

        CpuMock.Registers.A.Should().Be(expected_a);
        VerifyFlags(expected_flags);
    }

    public class Immediate : SBC_Tests
    {
        public Immediate(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new SBC_Immediate();

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte operand)
        {
            Memory[0x0000] = operand;
        }
    }

    public class Absolute : SBC_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new SBC_Absolute();

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte expectedValue)
        {
            Memory[0x0000] = 0x12;
            Memory[0x0001] = 0x34;

            Memory[0x3412] = expectedValue;
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0x45;

            Memory[0x4523] = 0x09;

            CpuMock.Registers.A = 0x0A;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x01);
        }
    }

    public class AbsoluteX : SBC_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new SBC_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xff;

            Memory[0x0105] = 0x06;

            CpuMock.Registers.X = 0x03;

            CpuMock.Registers.A = 0x20;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x1A);
        }

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0408] = value;

            CpuMock.Registers.X = 0x05;
        }

        [Fact]
        public void Should_require_4_cycles_when_x_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 4;

            Memory[0x0000] = 0x01;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0500] = 0x45;

            CpuMock.Registers.X = 0xFF;

            CpuMock.Registers.A = 0x69;
            CpuMock.Flags.C = true;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x24);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }

    public class AbsoluteY : SBC_Tests
    {
        public AbsoluteY(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new SBC_AbsoluteY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xff;

            Memory[0x0105] = 0x06;

            CpuMock.Registers.Y = 0x03;

            CpuMock.Registers.A = 0x0A;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x04);
        }

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0408] = value;

            CpuMock.Registers.Y = 0x05;
        }

        [Fact]
        public void Should_require_4_cycles_when_y_indexing_causes_page_transition()
        {
            CpuMock.State.RemainingCycles = 4;

            Memory[0x0000] = 0x01;
            Memory[0x0001] = 0x04;
            Memory[0x0002] = 0xff;

            Memory[0x0500] = 0x45;

            CpuMock.Registers.Y = 0xFF;

            CpuMock.Registers.A = 0x69;
            CpuMock.Flags.C = true;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x24);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }

    public class Zeropage : SBC_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new SBC_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0020] = 0x01;

            CpuMock.Registers.A = 0x02;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x01);
        }

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0xff;

            Memory[0x0003] = value;
        }
    }

    public class ZeropageX : SBC_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new SBC_ZeropageX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0031] = 0x01;

            CpuMock.Registers.X = 0x11;

            CpuMock.Registers.A = 0x03;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x02);
        }

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;

            Memory[0x0034] = value;

            CpuMock.Registers.X = 0x11;
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary()
        {
            Memory[0x0000] = 0xff;
            Memory[0x0001] = 0x09;
            Memory[0x0002] = 0xff;

            CpuMock.Registers.X = 0x02;

            CpuMock.Registers.A = 0x0A;
            CpuMock.Flags.C = true;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x01);
        }
    }

    public class IndirectX : SBC_Tests
    {
        public IndirectX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;
        protected override Instruction Sut { get; } = new SBC_IndirectX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0031] = 0x01;
            Memory[0x0032] = 0x02;

            Memory[0x0201] = 0x05;

            CpuMock.Registers.X = 0x11;

            CpuMock.Registers.A = 0x0A;
            CpuMock.Flags.C = true;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x05);
        }

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0034] = 0x01;
            Memory[0x0035] = 0x01;

            Memory[0x0101] = value;

            CpuMock.Registers.X = 0x11;
        }

        [Fact]
        public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary_when_fetching_indirect_address()
        {
            Memory[0x0000] = 0xff;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0x01;

            Memory[0x0101] = 0x08;

            CpuMock.Registers.X = 0x02;

            CpuMock.Registers.A = 0x0A;
            CpuMock.Flags.C = true;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x02);
        }
    }

    public class IndirectY : SBC_Tests
    {
        public IndirectY(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;
        protected override Instruction Sut { get; } = new SBC_IndirectY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x70;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0070] = 0x43;
            Memory[0x0071] = 0x35;

            Memory[0x3553] = 0x23;

            CpuMock.Registers.A = 0x25;
            CpuMock.Flags.C = true;

            CpuMock.Registers.Y = 0x10;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x02);
        }

        protected override void SBC_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0023] = 0x01;
            Memory[0x0024] = 0x02;

            Memory[0x0212] = value;

            CpuMock.Registers.Y = 0x11;
        }

        [Fact]
        public void Should_require_5_cycles_when_y_indexing_causes_page_transition()
        {
            Memory[0x0000] = 0x70;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0070] = 0x43;
            Memory[0x0071] = 0x35;

            Memory[0x3642] = 0x05;

            CpuMock.Registers.Y = 0xFF;

            CpuMock.Registers.A = 0x0A;
            CpuMock.Flags.C = true;

            CpuMock.State.RemainingCycles = 5;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x05);
            CpuMock.State.Ticks.Should().Be(5);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }
}
