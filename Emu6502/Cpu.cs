using System.Diagnostics;

namespace Emu6502;

public class Cpu
{
    public static class Instructions
    {
        public const byte Test = 0x02;
        public const byte LDA_Immediate = 0xA9;
        public const byte NOP = 0xEA;
    }

    public byte A { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public byte S { get; set; }
    public ushort PC { get; set; }

    public bool N { get; set; }
    public bool V { get; set; }
    public bool B { get; set; }
    public bool D { get; set; }
    public bool I { get; set; }
    public bool Z { get; set; }
    public bool C { get; set; }

    private int _ticks;
    private byte[] _memory;

    private Dictionary<byte, Action<Cpu>> _instructions = new()
    {
        { 
            Instructions.Test,
            (cpu) =>
            {
                Console.WriteLine("test instruction");
                cpu._ticks++;
            }
        },
        {
            Instructions.LDA_Immediate,
            (cpu) =>
            {
                cpu.A = cpu.FetchMemory();
                cpu.N = (cpu.A & 0b1000_0000) > 0;
                cpu.Z = cpu.A == 0;
            }
        },
        {
            Instructions.NOP,
            (cpu) =>
            {
                cpu._ticks++;
            }
        },
    };

    public Cpu(byte[] memory)
    {
        _memory = memory;
    }

    public void Reset()
    {
        A = 0;
        X = 0;
        Y = 0;
        S = 0;
        PC = 0xfffc;
        N = false;
        V = false;
        B = false;
        D = false;
        I = false;
        Z = false;
        C = false;
    }

    public void Execute(int cycles)
    {
        while (_ticks < cycles)
        {
            var inst = FetchMemory();
            try
            {
                _instructions[inst](this);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine(">> Undefined instruction");
                break;
            }            
        }
    }

    private byte FetchMemory()
    {
        var b = _memory[PC];
        PC++;
        _ticks++;
        return b;
    }
}