using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 六边形网格类
/// </summary>
public class HexGrid : MonoBehaviour
{
    // 网格块的个数
    public int chunkCountX = 4, chunkCountZ = 3;
    public Color defaultColor = Color.white;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;
    public Texture2D noiseSource;

    private HexGridChunk[] chunks;      // 所有网格块
    private HexCell[] cells;            // 所有cells
    private int cellCountX, cellCountZ; // cells的个数

    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    private void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    private void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void OnEnable()
    {
        // 防止在运行模式下重新编译后静态变量数据被清除
        HexMetrics.noiseSource = noiseSource;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);     // x方向的增量为六边形内圈的2倍
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);   // y方向的增量为六边形外圈的1.5倍

        HexCell cell = cells[i] = Instantiate(cellPrefab);
//        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]); // 从E到W连接Cell
        }

        if (z > 0)
        {
            if ((z & 1) == 0)   // 非0偶数排
            {
                // 设置SE方向上的相邻Cell
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    // 设置SW方向上的相邻Cell
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else    // 奇数排
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate(cellLabelPrefab);
//        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        cell.uiRect = label.rectTransform;
        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    public void ShowUI(bool visible)
    {
        foreach (HexGridChunk chunk in chunks)
        {
            chunk.ShowUI(visible);
        }
    }
}
