using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    private HexCell[] cells;    // 一个网格块中包含的cells
    private HexMesh hexMesh;
    private Canvas gridCanvas;

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        ShowUI(false);
    }

    public void Refresh()
    {
        enabled = true;
    }

    void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        enabled = false;
    }

    public void AddCell(int index, HexCell cell)
    {
        cells[index] = cell;
        cell.chunk = this;
        cell.transform.SetParent(transform, false);
        cell.uiRect.SetParent(gridCanvas.transform, false);
    }

    public void ShowUI(bool visible)
    {
        gridCanvas.gameObject.SetActive(visible);
    }
}
