namespace Emu6502.Tests.Unit.Instructions;
public abstract class ORA_Tests : InstructionTestBase
{
    public ORA_Tests(ITestOutputHelper output) : base(output) { }

    protected abstract void ORA_instruction_test_memory_setup(ICpu cpu, byte operand);

    [Theory]                     //nvdizc
    [InlineData(0x00, 0x01, 0x01, "000000")]
    [InlineData(0x01, 0x00, 0x01, "000000")]
    [InlineData(0x01, 0x01, 0x01, "000000")]
    [InlineData(0x11, 0x6E, 0x7F, "000000")]
    [InlineData(0x80, 0x20, 0xA0, "100000")]
    [InlineData(0x0F, 0xF0, 0xFF, "100000")]
    [InlineData(0x00, 0x00, 0x00, "000010")]
    public void Should_OR_A_with_memory_into_A_and_update_Z_and_N_flags_matching_result_stored_in_accumulator(
       byte inital_a,
       byte memory,
       byte expected_a,
       string expected_flags)
    {
        ORA_instruction_test_memory_setup(CpuMock, memory);

        CpuMock.Registers.A = inital_a;

        Sut.Execute(CpuMock);

        CpuMock.Registers.A.Should().Be(expected_a);
        CpuMock.Flags.N.Should().Be(expected_flags[0] == '1');
        CpuMock.Flags.V.Should().Be(expected_flags[1] == '1');
        CpuMock.Flags.Z.Should().Be(expected_flags[4] == '1');
        CpuMock.Flags.C.Should().Be(expected_flags[5] == '1');
    }

    public class Immediate : ORA_Tests
    {
        public Immediate(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 1;
        protected override Instruction Sut { get; } = new ORA_Immediate();

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte operand)
        {
            Memory[0x0000] = operand;
        }
    }

    public class Absolute : ORA_Tests
    {
        public Absolute(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ORA_Absolute();

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte operand)
        {
            Memory[0x0000] = 0x12;
            Memory[0x0001] = 0x34;

            Memory[0x3412] = operand;
        }

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x23;
            Memory[0x0001] = 0x45;

            Memory[0x4523] = 0x55;

            CpuMock.Registers.A = 0x22;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x77);
        }
    }

    public class AbsoluteX : ORA_Tests
    {
        public AbsoluteX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ORA_AbsoluteX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xff;

            Memory[0x0105] = 0x22;

            CpuMock.Registers.X = 0x03;
            CpuMock.Registers.A = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x33);
        }

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte value)
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

            Memory[0x0500] = 0x66;

            CpuMock.Registers.X = 0xFF;
            CpuMock.Registers.A = 0x11;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x77);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }

    public class AbsoluteY : ORA_Tests
    {
        public AbsoluteY(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ORA_AbsoluteY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x02;
            Memory[0x0001] = 0x01;
            Memory[0x0002] = 0xff;

            Memory[0x0105] = 0x06;

            CpuMock.Registers.Y = 0x03;
            CpuMock.Registers.A = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x17);
        }

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte value)
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

            Memory[0x0500] = 0x68;

            CpuMock.Registers.Y = 0xFF;
            CpuMock.Registers.A = 0x11;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x79);
            CpuMock.State.Ticks.Should().Be(4);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }

    public class Zeropage : ORA_Tests
    {
        public Zeropage(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 2;
        protected override Instruction Sut { get; } = new ORA_Zeropage();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0020] = 0x01;
            CpuMock.Registers.A = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x11);
        }

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte value)
        {
            Memory[0x0000] = 0x03;
            Memory[0x0001] = 0xff;

            Memory[0x0003] = value;
        }
    }

    public class ZeropageX : ORA_Tests
    {
        public ZeropageX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 3;
        protected override Instruction Sut { get; } = new ORA_ZeropageX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;

            Memory[0x0031] = 0x01;

            CpuMock.Registers.X = 0x11;
            CpuMock.Registers.A = 0x10;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x11);
        }

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte value)
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
            Memory[0x0001] = 0x65;
            Memory[0x0002] = 0xff;

            CpuMock.Registers.X = 0x02;
            CpuMock.Registers.A = 0x11;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x75);
        }
    }

    public class IndirectX : ORA_Tests
    {
        public IndirectX(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 5;
        protected override Instruction Sut { get; } = new ORA_IndirectX();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x20;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0031] = 0x01;
            Memory[0x0032] = 0x02;

            Memory[0x0201] = 0x05;

            CpuMock.Registers.X = 0x11;
            CpuMock.Registers.A = 0x11;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x15);
        }

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte value)
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

            Memory[0x0101] = 0x74;

            CpuMock.Registers.X = 0x02;
            CpuMock.Registers.A = 0x00;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x74);
        }
    }

    public class IndirectY : ORA_Tests
    {
        public IndirectY(ITestOutputHelper output) : base(output) { }

        public override int NumberOfCyclesForExecution => 4;
        protected override Instruction Sut { get; } = new ORA_IndirectY();

        public override void SteppedThroughSetup()
        {
            Memory[0x0000] = 0x70;
            Memory[0x0001] = 0xff;
            Memory[0x0002] = 0xff;

            Memory[0x0070] = 0x43;
            Memory[0x0071] = 0x35;

            Memory[0x3553] = 0x23;

            CpuMock.Registers.Y = 0x10;
            CpuMock.Registers.A = 0x10;
        }

        public override void SteppedThroughVerification()
        {
            CpuMock.Registers.A.Should().Be(0x33);
        }

        protected override void ORA_instruction_test_memory_setup(ICpu cpu, byte value)
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

            Memory[0x3642] = 0x78;

            CpuMock.Registers.Y = 0xFF;
            CpuMock.Registers.A = 0x01;

            CpuMock.State.RemainingCycles = 5;

            Sut.Execute(CpuMock);

            CpuMock.Registers.A.Should().Be(0x79);
            CpuMock.State.Ticks.Should().Be(5);
            CpuMock.State.Instruction.Should().BeNull();
        }
    }
}
