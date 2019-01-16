using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;      // 所在坐标
    public RectTransform uiRect;
    public HexGridChunk chunk;      // 所属大块

    [SerializeField]
    private HexCell[] neighbors;    // 相邻的Cell

    public int Elevation
    {
        get => elevation;
        set
        {
            if (elevation == value)
            {
                return;
            }
            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) *
                          HexMetrics.elevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = -position.y;
            uiRect.localPosition = uiPosition;

            // 当Elevation改变时，就是该刷新的时候
            Refresh();
        }
    }
    private int elevation = int.MinValue;   // 高度

    public Color Color
    {
        get => color;
        set
        {
            if (color == value)
            {
                return;
            }
            color = value;

            // 颜色改变时需要刷新
            Refresh();
        }
    }
    private Color color;    // 颜色

    public Vector3 Position => transform.localPosition;


    /// <summary>
    /// 根据方向获取相邻Cell
    /// </summary>
    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int) direction];
    }

    /// <summary>
    /// 根据方向设置相邻Cell
    /// </summary>
    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int) direction] = cell;

        // 顺便把自己也设为目标的相邻Cell
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    /// <summary>
    /// 获取边界类型，通过指定方向
    /// </summary>
    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int) direction].elevation);
    }

    /// <summary>
    /// 获取边界类型，通过指定Cell
    /// </summary>
    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    /// <summary>
    /// 刷新方法，当这个Cell需要被刷新的时候，直接刷新所属块
    /// </summary>
    private void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}
