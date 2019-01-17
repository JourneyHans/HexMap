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

    // 是否有流入河流/流出河流
    public bool HasIncomingRiver => hasIncomingRiver;
    public bool HasOutgoingRiver => hasOutgoingRiver;
    private bool hasIncomingRiver, hasOutgoingRiver;

    // 流入河流/流出河流方向
    public HexDirection IncomingRiver => incomingRiver;
    public HexDirection OutgoingRiver => outgoingRiver;
    private HexDirection incomingRiver, outgoingRiver;

    // Cell中是否包含河流
    public bool HasRiver => hasIncomingRiver || hasOutgoingRiver;

    public bool HasRiverBeginOrEnd => hasIncomingRiver != hasOutgoingRiver;

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return hasIncomingRiver && incomingRiver == direction ||
               hasOutgoingRiver && outgoingRiver == direction;
    }

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

            // 确保高度改变时，河流不会往高处走
            if (hasOutgoingRiver && elevation < GetNeighbor(outgoingRiver).elevation)
            {
                RemoveOutgoingRiver();
            }
            if (hasIncomingRiver && elevation > GetNeighbor(incomingRiver).elevation)
            {
                RemoveIncomingRiver();
            }

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
        if (!chunk) return;
        chunk.Refresh();
        foreach (HexCell neighbor in neighbors)
        {
            if (neighbor != null && neighbor.chunk != chunk)
            {
                neighbor.chunk.Refresh();
            }
        }
    }

    /// <summary>
    /// 刷新自己所属块
    /// </summary>
    private void RefreshSelfOnly()
    {
        chunk.Refresh();
    }

    /// <summary>
    /// 设置流出河流
    /// </summary>
    public void SetOutgoingRiver(HexDirection direction)
    {
        if (hasOutgoingRiver && outgoingRiver == direction)
        {
            // 已经存在
            return;
        }
        HexCell neighbor = GetNeighbor(direction);
        if (!neighbor || elevation < neighbor.elevation)
        {
            // 水不能往上流
            return;
        }

        RemoveOutgoingRiver();
        if (hasIncomingRiver && incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        hasOutgoingRiver = true;
        outgoingRiver = direction;
        RefreshSelfOnly();

        neighbor.RemoveIncomingRiver();
        neighbor.hasIncomingRiver = true;
        neighbor.incomingRiver = direction.Opposite();
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除流出河流
    /// </summary>
    public void RemoveOutgoingRiver()
    {
        if (!hasOutgoingRiver)
        {
            return;
        }
        hasOutgoingRiver = false;
        RefreshSelfOnly();

        // 移除了自己流出河流后，根据流出河流的方向获取相邻的Cell
        // 再移除相邻Cell的流入河流
        HexCell neighbor = GetNeighbor(outgoingRiver);
        neighbor.hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除流入河流
    /// </summary>
    public void RemoveIncomingRiver()
    {
        if (!hasIncomingRiver)
        {
            return;
        }
        hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(incomingRiver);
        neighbor.hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    /// <summary>
    /// 移除河流
    /// </summary>
    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }
}
