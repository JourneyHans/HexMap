/// <summary>
/// 六边形相邻的六个方向
/// </summary>
public enum HexDirection
{
    // 东北、东、东南、西南、西、西北
    NE, E, SE, SW, W, NW,
}

/// <summary>
/// 方向枚举扩展类
/// </summary>
public static class HexDirectionExtensions
{
    /// <summary>
    /// 取对面方向
    /// </summary>
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? direction + 3 : direction - 3;
    }

    /// <summary>
    /// 取上一个方向
    /// </summary>
    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : direction - 1;
    }

    /// <summary>
    /// 取下一个方向
    /// </summary>
    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : direction + 1;
    }
}
