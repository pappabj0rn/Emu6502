namespace Emu6502.Tests.Unit.Instructions;

public static class CpuExtensions
{
    public static int ActiveFlags(this ICpu cpu)
    {
        return (cpu.Flags.C ? 1 : 0)
            + (cpu.Flags.D ? 1 : 0)
            + (cpu.Flags.I ? 1 : 0)
            + (cpu.Flags.Z ? 1 : 0)
            + (cpu.Flags.N ? 1 : 0)
            + (cpu.Flags.V ? 1 : 0);
    }
}
