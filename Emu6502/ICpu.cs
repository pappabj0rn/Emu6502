﻿namespace Emu6502
{
    public interface ICpu
    {
        Flags Flags { get; }
        Registers Registers { get; }
        int Ticks { get; }
        ExecutionState State { get; }

        void Execute(int cycles);
        byte FetchMemory();
        void Reset();
    }
}