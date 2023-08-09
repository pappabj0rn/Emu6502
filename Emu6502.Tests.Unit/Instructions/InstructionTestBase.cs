﻿using Emu6502.Instructions;
namespace Emu6502.Tests.Unit.Instructions;

public abstract class InstructionTestBase
{
    protected ICpu CpuMock = Substitute.For<ICpu>();
    protected ExecutionState State = new();

    protected abstract Instruction Sut { get; }
    public abstract int NumberOfCyclesForExecution { get; }
    public virtual void SteppedThroughSetup() { }
    public virtual void SteppedThroughVerification() 
    {
        throw new NotImplementedException($"SteppedThroughVerification not implemeted for test of {Sut.GetType().FullName}");
    }
    
    public InstructionTestBase()
    {
        State.Instruction = Sut;

        CpuMock.State.Returns(State);

        CpuMock
            .FetchMemory()
            .ReturnsForAnyArgs((byte)0x00)
            .AndDoes(x =>State.Tick());

        CpuMock
            .FetchX()
            .Returns(x => CpuMock.Registers.X)
            .AndDoes(x => State.Tick());

        CpuMock
            .FetchY()
            .Returns(x => CpuMock.Registers.Y)
            .AndDoes(x => State.Tick());
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
}