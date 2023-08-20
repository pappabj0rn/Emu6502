using Emu6502.Instructions;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Emu6502.Tests.Unit.Instructions;

public abstract class InstructionTestBase
{
    protected ICpu CpuMock = Substitute.For<ICpu>();
    protected ExecutionState State = new();
    protected byte[] Memory = new byte[0x10000];

    private readonly ITestOutputHelper _output;

    protected abstract Instruction Sut { get; }
    public abstract int NumberOfCyclesForExecution { get; }
    public virtual void SteppedThroughSetup() { }
    public virtual void SteppedThroughVerification() 
    {
        throw new NotImplementedException($"SteppedThroughVerification not implemeted for test of {Sut.GetType().FullName}");
    }
    
    public InstructionTestBase(ITestOutputHelper output)
    {
        _output = output;

        CpuMock.Flags = new Flags();
        CpuMock.Flags.I = true;
        CpuMock.Registers.SP = 0xFF;

        State.Instruction = Sut;
        
        CpuMock.State.Returns(State);

        CpuMock
            .FetchMemory(Arg.Any<ushort?>())
            .Returns(x =>
            {
                return x[0] is null
                    ? Memory[CpuMock.Registers.PC++]
                    : Memory[(ushort)x[0]];
            })
            .AndDoes(x =>
            {
                ushort addr = x[0] is null
                    ? CpuMock.Registers.PC
                    : (ushort)x[0];
                var value = Memory[addr];
                _output.WriteLine($"[{CpuMock.State.Ticks}] R 0x{addr:X4} (0x{value:X2})");

                State.Tick();
            });

        CpuMock
            .When(x => x.WriteMemory(Arg.Any<byte>(), Arg.Any<ushort?>()))
            .Do(x => {
                State.Tick();
                var value = (byte)x[0];
                ushort addr = x[1] is null
                    ? CpuMock.Registers.PC++
                    : (ushort)x[1];

                Memory[addr] = value;
                _output.WriteLine($"[{CpuMock.State.Ticks}] W 0x{addr:X4} (0x{value:X2})");
            });

        CpuMock
            .When(x => x.SetRegister(Arg.Any<Register>(), Arg.Any<byte>()))
            .Do(x =>
            {
                var value = (byte)x[1];
                switch ((Register)x[0])
                {
                    case Register.A:
                        CpuMock.Registers.A = value;
                        break;
                    case Register.X:
                        CpuMock.Registers.X = value;
                        break;
                    case Register.Y:
                        CpuMock.Registers.Y = value;
                        break;
                    case Register.SP:
                        CpuMock.Registers.SP = value;
                        break;
                }

                CpuMock.UpdateNZ(value);
            });

        CpuMock
            .When(x => x.UpdateNZ(Arg.Any<byte>()))
            .Do(x => {
                var value = (byte)x[0];
                CpuMock.Flags.N = (value & 0x80) > 0;
                CpuMock.Flags.Z = value == 0;
            });
    }

    [Fact]
    public void Should_execute_in_defined_number_of_cycles()
    {
        State.RemainingCycles = NumberOfCyclesForExecution;

        Sut.Execute(CpuMock);

        State.RemainingCycles.Should().Be(0);
        State.Ticks.Should().Be(NumberOfCyclesForExecution);
        State.Instruction.Should().BeNull();
        State.InstructionSubstate.Should().Be(0);
    }

    [Fact]
    public void Should_be_able_to_be_stepped_through_when_requiring_more_than_one_cycle()
    {
        if (NumberOfCyclesForExecution == 1) return;

        SteppedThroughSetup();

        for (int i = 0; i < NumberOfCyclesForExecution; i++)
        {
            State.Instruction.Should().NotBeNull();
            State.RemainingCycles = 1;
            Sut.Execute(CpuMock);
        }

        SteppedThroughVerification();
    }

    protected void VerifyFlags(string expected_flags)
    {
        CpuMock.Flags.N.Should().Be(expected_flags[0] == '1');
        CpuMock.Flags.V.Should().Be(expected_flags[1] == '1');
        CpuMock.Flags.D.Should().Be(expected_flags[2] == '1');
        CpuMock.Flags.I.Should().Be(expected_flags[3] == '1');
        CpuMock.Flags.Z.Should().Be(expected_flags[4] == '1');
        CpuMock.Flags.C.Should().Be(expected_flags[5] == '1');
    }

    protected void VerifyFlags(byte expected_sr, bool ignoreSpecialBits = true)
    {
        byte expected = (byte)(ignoreSpecialBits 
            ? expected_sr | 0x30 
            : expected_sr);

        CpuMock.Flags.GetSR().Should().Be(expected);
    }
}