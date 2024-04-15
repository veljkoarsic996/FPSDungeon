
using Enums;

namespace Enums
{
    public enum RoomLayout
    {
        UDLR = 0,
        UDL = 1,
        UDR = 2,
        ULR = 3,
        DLR = 4,
        UD = 5,
        UL = 6,
        UR = 7,
        DL = 8,
        DR = 9,
        LR = 10,
        U = 11,
        D = 12,
        L = 13,
        R = 14,
        None = 15,
    }

    public enum Origin
    { 
        Up = 0, 
        Down = 1,
        Left = 2,
        Right = 3,
    }

}
[System.Serializable]
public class RoomConfig 
{
    public Origin CameFrom;
    public RoomLayout[] AvailableLayouts;
}

