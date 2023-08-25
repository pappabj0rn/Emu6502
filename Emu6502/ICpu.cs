namespace Emu6502;

public interface ICpu
{
    Pins Pins { get; set; }
    Flags Flags { get; set; }
    Registers Registers { get; set; }
    int Ticks { get; }
    ExecutionState State { get; }

    void Execute(int cycles);
    byte FetchMemory(ushort? addr = null);
    void WriteMemory(byte value, ushort? addr = null);
    void Reset();
    void SetRegister(Register register, byte value);
    void UpdateNZ(byte value);
}