using UnityEngine;

/// <summary>
/// 六边形网格坐标结构体
/// </summary>
[System.Serializable]
public struct HexCoordinates
{
    [SerializeField]
    private int x, z;

    public int X => x;      // "—" ←X代表水平方向
    public int Z => z;      // "\"  ←Z代表这个方向
    public int Y => -X - Z; // "/"  ←Y代表这个方向

    public HexCoordinates(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        return new HexCoordinates(x - z / 2, z);
    }

    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x / (HexMetrics.innerRadius * 2f);
        float y = -x;
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        x -= offset;
        y -= offset;
        int iX = Mathf.RoundToInt(x);
        int iY = Mathf.RoundToInt(y);
        int iZ = Mathf.RoundToInt(-x - y);
        if (iX + iY + iZ != 0)
        {
            float dX = Mathf.Abs(x - iX);
            float dY = Mathf.Abs(y - iY);
            float dZ = Mathf.Abs(-x - y - iZ);
            if (dX > dY && dX > dZ)
            {
                iX = -iY - iZ;
            }
            else if (dZ > dY)
            {
                iZ = -iX - iY;
            }
        }
        return new HexCoordinates(iX, iZ);
    }

    public override string ToString()
    {
        return "(" + X + ", " + Y + ", " + Z + ")";
    }

    public string ToStringOnSeparateLines()
    {
        return X + "\n" + Y + "\n" + Z;
    }
}