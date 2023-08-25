namespace Emu6502;

public class Pins
{
    public bool Rdy;
    public bool Phi;
    public bool PhiOut1;
    public bool PhiOut2;
    public bool IRQ_;
    public bool NMI_;
    public bool Sync;
    public bool A0;
    public bool A1;
    public bool A2;
    public bool A3;
    public bool A4;
    public bool A5;
    public bool A6;
    public bool A7;
    public bool A8;
    public bool A9;
    public bool A10;
    public bool A11;
    public bool A12;
    public bool A13;
    public bool A14;
    public bool A15;
    public bool D0;
    public bool D1;
    public bool D2;
    public bool D3;
    public bool D4;
    public bool D5;
    public bool D6;
    public bool D7;
    public bool RW;
    public bool Res;

    public void SetAddr(ushort addr)
    {
        A0 =  (addr & 0x0001) > 0;
        A1 =  (addr & 0x0002) > 0;
        A2 =  (addr & 0x0004) > 0;
        A3 =  (addr & 0x0008) > 0;
        A4 =  (addr & 0x0010) > 0;
        A5 =  (addr & 0x0020) > 0;
        A6 =  (addr & 0x0040) > 0;
        A7 =  (addr & 0x0080) > 0;
        A8 =  (addr & 0x0100) > 0;
        A9 =  (addr & 0x0200) > 0;
        A10 = (addr & 0x0400) > 0;
        A11 = (addr & 0x0800) > 0;
        A12 = (addr & 0x1000) > 0;
        A13 = (addr & 0x2000) > 0;
        A14 = (addr & 0x4000) > 0;
        A15 = (addr & 0x8000) > 0;
    }

    public ushort GetAddr()
    {
        return (ushort)(
               (A0 ? 1 : 0)
            | ((A1  ? 1 : 0) <<  1)
            | ((A2  ? 1 : 0) <<  2)
            | ((A3  ? 1 : 0) <<  3)
            | ((A4  ? 1 : 0) <<  4)
            | ((A5  ? 1 : 0) <<  5)
            | ((A6  ? 1 : 0) <<  6)
            | ((A7  ? 1 : 0) <<  7)
            | ((A8  ? 1 : 0) <<  8)
            | ((A9  ? 1 : 0) <<  9)
            | ((A10 ? 1 : 0) << 10)
            | ((A11 ? 1 : 0) << 11)
            | ((A12 ? 1 : 0) << 12)
            | ((A13 ? 1 : 0) << 13)
            | ((A14 ? 1 : 0) << 14)
            | ((A15 ? 1 : 0) << 15)
        );
    }

    public void SetData(byte data)
    {
        D0 = (data & 0x01) > 0;
        D1 = (data & 0x02) > 0;
        D2 = (data & 0x04) > 0;
        D3 = (data & 0x08) > 0;
        D4 = (data & 0x10) > 0;
        D5 = (data & 0x20) > 0;
        D6 = (data & 0x40) > 0;
        D7 = (data & 0x80) > 0;
    }

    public byte GetData()
    {
        return (byte)(
               (D0 ? 1 : 0)
            | ((D1 ? 1 : 0) << 1)
            | ((D2 ? 1 : 0) << 2)
            | ((D3 ? 1 : 0) << 3)
            | ((D4 ? 1 : 0) << 4)
            | ((D5 ? 1 : 0) << 5)
            | ((D6 ? 1 : 0) << 6)
            | ((D7 ? 1 : 0) << 7)
        );
    }


    public bool IsReading => RW;
    public bool IsWriting => !RW;
}
