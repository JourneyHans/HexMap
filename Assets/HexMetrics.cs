using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六边形边界类型
/// </summary>
public enum HexEdgeType
{
    // 平坦、坡度、悬崖
    Flat, Slope, Cliff
}

/// <summary>
/// 六边形度量单位类
/// </summary>
public static class HexMetrics
{
    // 外圈半径设定为10f
    public const float outerRadius = 10f;

    // 内圈半径 = 外圈半径 * (√3/2)
    // 另外，两个六边形的圆心之间的距离，正好是内圈半径的两倍
    public const float innerRadius = outerRadius * 0.866025404f;

    // 单色占比
    public const float solidFactor = 0.8f;
    // 混合色占比
    public const float blendFactor = 1f - solidFactor;
    // Cell高度步长
    public const float elevationStep = 3f;

    // 每个单位Slope包含两个台阶
    public const int terracesPerSlope = 2;
    // 台阶步长：每个单位Slope包含5个部分（2个平坦的+3个斜坡）
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    // 水平方向上，每个步长的大小为1/5
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

    // 噪点贴图
    public static Texture2D noiseSource;
    // 干扰力度
    public const float cellPerturbStrength = 4f;
    // 噪点缩放
    public const float noiseScale = 0.003f;
    // 高度干扰力度
    public const float elevationPerturbStrength = 1.5f;

    public const int chunkSizeX = 5, chunkSizeZ = 5;

    // 六边形六个角的坐标
    private static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius),   // 避免越界，重复第一个角
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int) direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int) direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int) direction] + corners[(int) direction + 1]) 
               //* 0.5f ← 优化网格，两个相连的六边形实际上用一个矩形即可连接，不需要两个
               * blendFactor;
    }

    /// <summary>
    /// 台阶顶点插值
    /// </summary>
    public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        float h = step * horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    /// <summary>
    /// 台阶颜色差值
    /// </summary>
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    /// <summary>
    /// 获取边界类型
    /// </summary>
    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        if (elevation1 == elevation2)
        {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1)
        {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }

    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
    }
}
