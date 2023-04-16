using System;
using UnityEngine;

[Flags]
public enum Side
{
    Invalid =            0x00000000,
    PositiveX =          0x00000001,
    NegativeX =          0x00000002,
    PositiveZ =          0x00000004,
    NegativeZ =          0x00000008,
    /*PositiveXPositiveZ = 0x00000010,
    PositiveXNegativeZ = 0x00000020,
    NegativeXPositiveZ = 0x00000040,
    NegativeXNegativeZ = 0x00000080,*/
    SideCount = 4,
}

public static class EnumExtensions
{
    public static Side GetInverseSide(this Side s)
    {
        switch (s)
        {
            case Side.PositiveX:
                return Side.NegativeX;
            case Side.NegativeX:
                return Side.PositiveX;
            case Side.PositiveZ:
                return Side.NegativeZ;
            case Side.NegativeZ:
                return Side.PositiveZ;
            /*case Side.PositiveXPositiveZ:
                return Side.NegativeXNegativeZ;
            case Side.PositiveXNegativeZ:
                return Side.NegativeXPositiveZ;
            case Side.NegativeXPositiveZ:
                return Side.PositiveXNegativeZ;
            case Side.NegativeXNegativeZ:
                return Side.PositiveXPositiveZ;*/
        }

        Debug.Assert(false, "Invalid Side!");
        return Side.Invalid;//Dummy return value
    }

    public static bool HasFlagFast(this Enum e, Enum test)
    {
        return ((int)(object)e & (int)(object)test) != 0;
    }
}

public class TileScript : MonoBehaviour
{
    public Side[] PassableSides;
    private Side _passableSideFlags = Side.Invalid;//used for faster check
    internal TileIndex Pos;

	void Start()
    {
        
	}

    public void InitPassableSideFlags()
    {
        if (_passableSideFlags == Side.Invalid)
        {
            if (PassableSides != null)
            {
                for (int i = 0; i < PassableSides.Length; ++i)
                {
                    _passableSideFlags |= PassableSides[i];
                }
            }

            Debug.Assert(_passableSideFlags > 0, "Tile is not passable at all!");
        }
    }

    public bool IsPassableToSide(Side s)
    {
        return _passableSideFlags.HasFlagFast(s);
    }

	void Update()
    {

	}
}