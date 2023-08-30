namespace Emu6502.Tests.Unit.Instructions;

public abstract class Compare_Tests : InstructionTestBase
{
    protected Compare_Tests(ITestOutputHelper output) : base(output)
    { }

    protected abstract void CompareTestSetup(byte mem);
    protected abstract void SetComparedRegister(byte value);
    protected abstract byte GetComparedRegister();

    [Theory]                //NV-BDIZC
    [InlineData(0x34, 0x34, 0b00110111)]    //Eq
    [InlineData(0xFF, 0xFF, 0b00110111)]
    [InlineData(0x00, 0x00, 0b00110111)]

    [InlineData(0x00, 0x01, 0b10110100)]    //Gt    
    [InlineData(0x81, 0x82, 0b10110100)]
    [InlineData(0x7E, 0x7F, 0b10110100)]
    
    [InlineData(0x7E, 0x7D, 0b00110101)]    //Lt
    [InlineData(0x81, 0x80, 0b00110101)]
    public void Should_compare_specified_memory_with_accumulator_and_set_Z_N_and_C(
        byte reg,
        byte mem,
        byte expectedFlags)
    {
        SetComparedRegister(reg);
        CompareTestSetup(mem);

        Sut.Execute(CpuMock);

        GetComparedRegister().Should().Be(reg);
        CpuMock.Flags.GetSR().Should().Be(expectedFlags);
    }
    public abstract class CMP_tests : Compare_Tests
    {
        protected CMP_tests(ITestOutputHelper output) : base(output) { }

        protected override void SetComparedRegister(byte value)
        {
            CpuMock.Registers.A = value;
        }

        protected override byte GetComparedRegister()
        {
            return CpuMock.Registers.A;
        }

        public class Immediate : CMP_tests
        {
            public Immediate(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 1;

            protected override Instruction Sut { get; } = new CMP_Immediate();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                Memory[0x0000] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = mem;
            }
        }

        public class Zeropage : CMP_tests
        {
            public Zeropage(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 2;

            protected override Instruction Sut { get; } = new CMP_Zeropage();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                Memory[0x0000] = 0x05;
                Memory[0x0005] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = 0x11;
                Memory[0x0011] = mem;
            }
        }

        public class ZeropageX : CMP_tests
        {
            public ZeropageX(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 3;

            protected override Instruction Sut { get; } = new CMP_ZeropageX();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.X = 0x11;
                Memory[0x0000] = 0x05;
                Memory[0x0016] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                CpuMock.Registers.X = 0x11;
                Memory[0x0000] = 0x11;
                Memory[0x0022] = mem;
            }
        }

        public class Absolute : CMP_tests
        {
            public Absolute(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 3;

            protected override Instruction Sut { get; } = new CMP_Absolute();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                Memory[0x0000] = 0x05;
                Memory[0x0001] = 0x06;
                Memory[0x0605] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = 0x11;
                Memory[0x0001] = 0x22;
                Memory[0x2211] = mem;
            }
        }

        public class AbsoluteX : CMP_tests
        {
            public AbsoluteX(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 3;

            protected override Instruction Sut { get; } = new CMP_AbsoluteX();


            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.X = 0x11;
                Memory[0x0000] = 0x05;
                Memory[0x0001] = 0x06;
                Memory[0x0616] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                CpuMock.Registers.X = 0x11;
                Memory[0x0000] = 0x11;
                Memory[0x0001] = 0x22;
                Memory[0x2222] = mem;
            }

            [Fact]
            public void Should_require_4_cycles_when_x_indexing_causes_page_transition()
            {
                CpuMock.State.RemainingCycles = 4;

                Memory[0x0000] = 0x01;
                Memory[0x0001] = 0x04;
                Memory[0x0002] = 0xff;

                Memory[0x0500] = 0x10;

                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.X = 0xFF;

                Sut.Execute(CpuMock);

                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
                CpuMock.State.Ticks.Should().Be(4);
                CpuMock.State.Instruction.Should().BeNull();
            }
        }

        public class AbsoluteY : CMP_tests
        {
            public AbsoluteY(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 3;

            protected override Instruction Sut { get; } = new CMP_AbsoluteY();


            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.Y = 0x11;
                Memory[0x0000] = 0x05;
                Memory[0x0001] = 0x06;
                Memory[0x0616] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                CpuMock.Registers.Y = 0x11;
                Memory[0x0000] = 0x11;
                Memory[0x0001] = 0x22;
                Memory[0x2222] = mem;
            }

            [Fact]
            public void Should_require_4_cycles_when_x_indexing_causes_page_transition()
            {
                CpuMock.State.RemainingCycles = 4;

                Memory[0x0000] = 0x01;
                Memory[0x0001] = 0x04;
                Memory[0x0002] = 0xff;

                Memory[0x0500] = 0x10;

                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.Y = 0xFF;

                Sut.Execute(CpuMock);

                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
                CpuMock.State.Ticks.Should().Be(4);
                CpuMock.State.Instruction.Should().BeNull();
            }
        }

        public class IndirectX : CMP_tests
        {
            public IndirectX(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 5;

            protected override Instruction Sut { get; } = new CMP_IndirectX();


            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.X = 0x01;
                Memory[0x0000] = 0x05;

                Memory[0x0006] = 0x06;
                Memory[0x0007] = 0x06;

                Memory[0x0606] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                CpuMock.Registers.X = 0x11;
                Memory[0x0000] = 0x11;

                Memory[0x0022] = 0x22;
                Memory[0x0023] = 0x22;

                Memory[0x2222] = mem;
            }

            [Fact]
            public void Should_wrap_around_to_start_of_zeropage_instead_of_crossing_page_boundary_when_fetching_indirect_address()
            {
                CpuMock.Registers.A = 0x10;
                Memory[0x0000] = 0xff;
                Memory[0x0001] = 0x01;
                Memory[0x0002] = 0x01;

                Memory[0x0101] = 0x10;

                CpuMock.Registers.X = 0x02;

                Sut.Execute(CpuMock);

                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }
        }

        public class IndirectY : CMP_tests
        {
            public IndirectY(ITestOutputHelper output) : base(output)
            { }

            public override int NumberOfCyclesForExecution => 4;

            protected override Instruction Sut { get; } = new CMP_IndirectY();


            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.Y = 0x01;
                Memory[0x0000] = 0x05;

                Memory[0x0005] = 0x06;
                Memory[0x0006] = 0x06;

                Memory[0x0607] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                CpuMock.Registers.Y = 0x11;
                Memory[0x0000] = 0x11;
                Memory[0x0011] = 0x22;
                Memory[0x0012] = 0x22;
                Memory[0x2233] = mem;
            }

            [Fact]
            public void Should_require_5_cycles_when_y_indexing_causes_page_transition()
            {
                Memory[0x0000] = 0x70;
                Memory[0x0001] = 0xff;
                Memory[0x0002] = 0xff;

                Memory[0x0070] = 0x43;
                Memory[0x0071] = 0x35;

                Memory[0x3642] = 0x10;

                CpuMock.Registers.A = 0x10;
                CpuMock.Registers.Y = 0xFF;

                CpuMock.State.RemainingCycles = 5;

                Sut.Execute(CpuMock);

                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
                CpuMock.State.Ticks.Should().Be(5);
                CpuMock.State.Instruction.Should().BeNull();
            }
        }
    }

    public abstract class CPX_tests : Compare_Tests
    {
        protected CPX_tests(ITestOutputHelper output) : base(output) { }

        protected override void SetComparedRegister(byte value)
        {
            CpuMock.Registers.X = value;
        }

        protected override byte GetComparedRegister()
        {
            return CpuMock.Registers.X;
        }

        public class Immediate : CPX_tests
        {
            public Immediate(ITestOutputHelper output) : base(output) { }

            public override int NumberOfCyclesForExecution => 1;

            protected override Instruction Sut { get; } = new CPX_Immediate();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.X = 0x10;
                Memory[0x0000] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = mem;
            }
        }

        public class Zeropage : CPX_tests
        {
            public Zeropage(ITestOutputHelper output) : base(output) { }

            public override int NumberOfCyclesForExecution => 2;

            protected override Instruction Sut { get; } = new CPX_Zeropage();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.X = 0x10;
                Memory[0x0000] = 0x05;
                Memory[0x0005] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = 0x11;
                Memory[0x0011] = mem;
            }
        }

        public class Absolute : CPX_tests
        {
            public Absolute(ITestOutputHelper output) : base(output) { }

            public override int NumberOfCyclesForExecution => 3;

            protected override Instruction Sut { get; } = new CPX_Absolute();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.X = 0x10;
                Memory[0x0000] = 0x05;
                Memory[0x0001] = 0x06;
                Memory[0x0605] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = 0x11;
                Memory[0x0001] = 0x22;
                Memory[0x2211] = mem;
            }
        }
    }

    public abstract class CPY_tests : Compare_Tests
    {
        protected CPY_tests(ITestOutputHelper output) : base(output) { }

        protected override void SetComparedRegister(byte value)
        {
            CpuMock.Registers.Y = value;
        }

        protected override byte GetComparedRegister()
        {
            return CpuMock.Registers.Y;
        }

        public class Immediate : CPY_tests
        {
            public Immediate(ITestOutputHelper output) : base(output) { }

            public override int NumberOfCyclesForExecution => 1;

            protected override Instruction Sut { get; } = new CPY_Immediate();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.Y = 0x10;
                Memory[0x0000] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = mem;
            }
        }

        public class Zeropage : CPY_tests
        {
            public Zeropage(ITestOutputHelper output) : base(output) { }

            public override int NumberOfCyclesForExecution => 2;

            protected override Instruction Sut { get; } = new CPY_Zeropage();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.Y = 0x10;
                Memory[0x0000] = 0x05;
                Memory[0x0005] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = 0x11;
                Memory[0x0011] = mem;
            }
        }

        public class Absolute : CPY_tests
        {
            public Absolute(ITestOutputHelper output) : base(output) { }

            public override int NumberOfCyclesForExecution => 3;

            protected override Instruction Sut { get; } = new CPY_Absolute();

            public override void SteppedThroughSetup()
            {
                CpuMock.Registers.Y = 0x10;
                Memory[0x0000] = 0x05;
                Memory[0x0001] = 0x06;
                Memory[0x0605] = 0x10;
            }

            public override void SteppedThroughVerification()
            {
                CpuMock.Flags.Z.Should().BeTrue();
                CpuMock.Flags.C.Should().BeTrue();
                CpuMock.Flags.N.Should().BeFalse();
            }

            protected override void CompareTestSetup(byte mem)
            {
                Memory[0x0000] = 0x11;
                Memory[0x0001] = 0x22;
                Memory[0x2211] = mem;
            }
        }
    }
}
