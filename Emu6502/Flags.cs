namespace Emu6502;

public class Flags
{
    public bool N;
    public bool V;

    public bool B;
    public bool D;
    public bool I;
    public bool Z;
    public bool C;

    /// <summary>
    /// Update flags to match given SR byte. Bits [6:5] are ignored
    /// Flag order: NV__DIZC
    /// </summary>
    /// <param name="sr"></param>
    public void SetSR(byte sr)
    {
        N = (sr & 0b1000_0000) > 0;
        V = (sr & 0b0100_0000) > 0;
        D = (sr & 0b0000_1000) > 0;
        I = (sr & 0b0000_0100) > 0;
        Z = (sr & 0b0000_0010) > 0;
        C = (sr & 0b0000_0001) > 0;
    }

    /// <summary>
    /// Get SR byte matching flags statuses. Bit [6:5] are allways returned as set.
    /// Flag order: NV__DIZC
    /// </summary>
    /// <returns></returns>
    public byte GetSR()
    {
        byte sr = (byte)(0b0011_0000
               | (N ? 0b1000_0000 : 0x00)
               | (V ? 0b0100_0000 : 0x00)
               | (D ? 0b0000_1000 : 0x00)
               | (I ? 0b0000_0100 : 0x00)
               | (Z ? 0b0000_0010 : 0x00)
               | (C ? 0b0000_0001 : 0x00));
        return sr;        
    }
}
